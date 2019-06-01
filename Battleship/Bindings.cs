using Ninject.Modules;

namespace Battleship
{
    public class Bindings : NinjectModule
    {
        public override void Load()
        {
            Bind<IBattleshipService>().To<BattleshipService>();
        }
    }
}