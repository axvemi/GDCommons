using Godot;

namespace Axvemi.Commons.Modules;

public partial class OutlineModule<T> : Node, IModule<T>
{
    public ModuleController<T> ModuleController { get; set; }
    public float Thickness { get; set; }
    public Color Color { get; set; }

    protected ShaderMaterial ShaderMaterial;

    public virtual void OnModulesReady()
    {
    }

    public void Highlight()
    {
        ShaderMaterial.SetShaderParameter("outline_thickness", Thickness);
        ShaderMaterial.SetShaderParameter("outline_color", Color);
    }

    public void ResetHighlight()
    {
        ShaderMaterial.SetShaderParameter("outline_thickness", 0);
    }
}
