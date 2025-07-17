using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
using static Godot.Control;

namespace GodotUtils.Visualize;

/// <summary>
/// Methods that can be executed manually by pressing a button in-game. Parameters are supported.
/// </summary>
public static class VisualMethods
{
    /// <summary>
    /// Creates the UI needed for the method parameters
    /// </summary>
    public static HBoxContainer CreateMethodParameterControls(MethodInfo method, List<VisualSpinBox> debugExportSpinBoxes, object[] providedValues)
    {
        HBoxContainer hboxParams = new();

        ParameterInfo[] paramInfos = method.GetParameters();

        for (int i = 0; i < paramInfos.Length; i++)
        {
            ParameterInfo paramInfo = paramInfos[i];
            Type paramType = paramInfo.ParameterType;

            providedValues[i] = CreateDefaultValue(paramType);

            int capturedIndex = i;

            VisualControlInfo control = VisualControlTypes.CreateControlForType(paramType, null, new VisualControlContext(debugExportSpinBoxes, providedValues[i], v =>
            {
                providedValues[capturedIndex] = v;
            }));

            if (control.VisualControl != null)
            {
                hboxParams.AddChild(new Label { Text = paramInfo.Name.ToPascalCase().AddSpaceBeforeEachCapital() });
                hboxParams.AddChild(control.VisualControl.Control);
            }
        }

        return hboxParams;
    }

    public static void AddMethodInfoElements(VBoxContainer vbox, IEnumerable<MethodInfo> methods, Node node, List<VisualSpinBox> debugExportSpinBoxes)
    {
        foreach (MethodInfo method in methods)
        {
            if (method.DeclaringType.IsSubclassOf(typeof(GodotObject)))
            {
                ParameterInfo[] paramInfos = method.GetParameters();
                object[] providedValues = new object[paramInfos.Length];

                HBoxContainer hboxParams = CreateMethodParameterControls(method, debugExportSpinBoxes, providedValues);

                vbox.AddChild(hboxParams);

                Button button = CreateMethodButton(method, node, paramInfos, providedValues);

                vbox.AddChild(button);
            }
        }
    }

    public static Button CreateMethodButton(MethodInfo method, object target, ParameterInfo[] paramInfos, object[] providedValues)
    {
        Button button = new()
        {
            Text = method.Name,
            SizeFlagsHorizontal = SizeFlags.ShrinkCenter
        };

        button.Pressed += () =>
        {
            object[] parameters = ParameterConverter.ConvertParameterInfoToObjectArray(paramInfos, providedValues);

            method.Invoke(target, parameters);
        };

        return button;
    }

    public static object CreateDefaultValue(Type type)
    {
        // Examples of Value Types: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-types#kinds-of-value-types-and-type-constraints
        if (type.IsValueType)
        {
            return Activator.CreateInstance(type);
        }
        // Examples of Reference Types: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/reference-types
        else if (type == typeof(string))
        {
            return string.Empty;
        }

        return null;
    }
}
