using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Zarya.SourceGenerators.Input;

[Generator]
public class InputGenerator : IIncrementalGenerator
{
	private const string KeyboardInputAttributeName = "Zarya.Input.KeyboardInputAttribute";
	private const string GamepadButtonInputAttributeName = "Zarya.Input.GamepadButtonInputAttribute";
	private const string MouseInputAttributeName = "Zarya.Input.MouseInputAttribute";
	private const string KeyboardAxisInputAttributeName = "Zarya.Input.KeyboardAxisInputAttribute";
	private const string GamepadAxisInputAttributeName = "Zarya.Input.GamepadAxisInputAttribute";

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var inputMethods = context.SyntaxProvider.CreateSyntaxProvider(
			(n, _) => n.IsKind(SyntaxKind.MethodDeclaration),
			(ctx, _) => ctx.SemanticModel.GetDeclaredSymbol(ctx.Node) as IMethodSymbol)
			// .Where(m => m is { IsPartialDefinition: true })
			.Select((m, _) => (method: m!, attributes: m!.GetAttributes().Where(a => a.AttributeClass?.ToDisplayString() is KeyboardInputAttributeName or GamepadButtonInputAttributeName or MouseInputAttributeName)))
			.Where(p => p.attributes.Any());
		context.RegisterSourceOutput(inputMethods, ProduceInputSourceOutput);

		var axisInputMethods = context.SyntaxProvider.CreateSyntaxProvider(
			(n, _) => n.IsKind(SyntaxKind.MethodDeclaration),
			(ctx, _) => ctx.SemanticModel.GetDeclaredSymbol(ctx.Node) as IMethodSymbol)
			// .Where(m => m is { IsPartialDefinition: true })
			.Select((m, _) => (method: m!, attributes: m!.GetAttributes().Where(a => a.AttributeClass?.ToDisplayString() is KeyboardAxisInputAttributeName or GamepadAxisInputAttributeName)))
			.Where(p => p.attributes.Any());
		context.RegisterSourceOutput(axisInputMethods, ProduceAxisInputSourceOutput);
	}

	private static void ProduceInputSourceOutput(SourceProductionContext context, (IMethodSymbol, IEnumerable<AttributeData>) element)
	{
		var (method, attributes) = element;

		var namespaceName = method.ContainingNamespace.Name;
		var className = method.ContainingType.Name;
		var methodName = method.Name;

		var attributeExpressions = attributes.Select(a => a switch
		{
			{ ConstructorArguments: [var key, var state] } when a.AttributeClass?.ToDisplayString() is KeyboardInputAttributeName =>
				$"InputManager.IsKey((global::Zarya.Key){key.Value}, (global::Zarya.InputState){state.Value})",
			{ ConstructorArguments: [var button, var state] } when a.AttributeClass?.ToDisplayString() is GamepadButtonInputAttributeName =>
				$"""
					(Player is global::Zarya.Input.InputBase.AnyGamepad
						? InputManager.IsButton((global::Zarya.Button){button.Value}, (global::Zarya.InputState){state.Value})
						: InputManager.IsButton((global::Zarya.Button){button.Value}, (global::Zarya.InputState){state.Value}, Player))
					""",
			{ ConstructorArguments: [var button, var state] } when a.AttributeClass?.ToDisplayString() is MouseInputAttributeName =>
				$"InputManager.IsMouseButton((global::Zarya.MouseButton){button.Value}, (global::Zarya.InputState){state.Value})",
			_ => throw new Exception("Unreachable state"),
		});

		context.AddSource(
			$"{namespaceName}.{className}.{methodName}.g.cs",
			$$"""
				namespace {{namespaceName}};

				partial class {{className}}
				{
					{{method.DeclaredAccessibility.ToString().ToLowerInvariant()}} partial bool {{methodName}}() =>
						{{string.Join(" || ", attributeExpressions)}};
				}
				""");
	}

	private static void ProduceAxisInputSourceOutput(SourceProductionContext context, (IMethodSymbol, IEnumerable<AttributeData>) element)
	{
		var (method, attributes) = element;

		var namespaceName = method.ContainingNamespace.Name;
		var className = method.ContainingType.Name;
		var methodName = method.Name;

		var attributeExpressions = attributes.Select(a => a switch
		{
			{ ConstructorArguments: [var negative, var positive] } when a.AttributeClass?.ToDisplayString() is KeyboardAxisInputAttributeName =>
				$$"""
					((InputManager.IsKeyPressed((global::Zarya.Key){{negative.Value}}), InputManager.IsKeyPressed((global::Zarya.Key){{positive.Value}})) switch
					{
						(true, false) => -1.0f,
						(false, true) => 1.0f,
						_ => 0.0f,
					})
					""",
			{ ConstructorArguments: [var axis] } when a.AttributeClass?.ToDisplayString() is GamepadAxisInputAttributeName =>
				$"""
					(Player is global::Zarya.Input.InputBase.AnyGamepad
						? InputManager.GetGamepadAxis((global::Zarya.GamepadAxis){axis.Value})
						: InputManager.GetGamepadAxis((global::Zarya.GamepadAxis){axis.Value}, Player))
					""",
			_ => throw new Exception("Unreachable state"),
		});

		context.AddSource(
			$"{namespaceName}.{className}.{methodName}.g.cs",
			$$"""
				namespace {{namespaceName}};

				partial class {{className}}
				{
					{{method.DeclaredAccessibility.ToString().ToLowerInvariant()}} partial float {{methodName}}() =>
						global::System.MathF.Max(-1.0f, global::System.MathF.Min({{string.Join(" + ", attributeExpressions)}}, 1.0f));
				}
				""");
	}
}
