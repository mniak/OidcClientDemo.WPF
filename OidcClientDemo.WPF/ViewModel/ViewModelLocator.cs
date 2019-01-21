using SimpleInjector;
using System.Net.Http;

namespace OidcClientDemo.WPF.ViewModel
{
    class ViewModelLocator
    {
        private static readonly Container container;

        static ViewModelLocator()
        {
            container = new Container();
            container.RegisterSingleton(() => new HttpClient());
            container.Register<MainWindowViewModel>();
        }
        public ViewModelLocator()
        {
            this.Main = container.GetInstance<MainWindowViewModel>();
        }
        public MainWindowViewModel Main { get; private set; }
    }
}
