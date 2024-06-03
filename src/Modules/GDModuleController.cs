using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Axvemi.Commons.Modules;

public partial class GDModuleController<T> : ModuleController<T> where T : Node
{
	public GDModuleController(T owner) : base(owner, GetModules(owner), GetProcessors(owner))
	{
		Modules = Owner.GetNode("Modules").GetChildren().OfType<IModule<T>>().ToList();
		Processors = Owner.GetNode("Modules").GetChildren().OfType<IProcessor<T>>().ToList();
	}

	private static List<IModule<T>> GetModules(T owner) => owner.GetNode("Modules").GetChildren().OfType<IModule<T>>().ToList();
	private static List<IProcessor<T>> GetProcessors(T owner) => owner.GetNode("Modules").GetChildren().OfType<IProcessor<T>>().ToList();
}