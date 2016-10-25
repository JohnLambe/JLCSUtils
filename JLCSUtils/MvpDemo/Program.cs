using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;

using SimpleInjector;
using DiExtension.SimpleInject;
using DiExtension.ConfigInject.Providers;
using MvpFramework;
using MvpDemo.Model;
using MvpFramework.Binding;
using DiExtension;
using JohnLambe.Util.Reflection;

namespace MvpDemo
{
    public class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            new Program().Run();
        }

        public void Run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SetupDi();

            var mainForm = DiContext.GetInstance<MainForm>();

            Application.Run(mainForm);
        }

        protected void SetupDi()
        {
            var assemblies = AssemblyUtils.GetReferencedAssemblies(Assembly.GetEntryAssembly(),true);
            
            Resolver = new DiMvpResolver(DiContext);
            DiContext.Container.RegisterSingleton<IDiResolver>(DiContext);
            DiContext.Container.RegisterSingleton(Resolver);
            //                typeof(MvpResolver),Resolver);

            new RegistrationHelper(Resolver, DiContext).ScanAssemblies(assemblies.ToArray()); // Assembly.GetExecutingAssembly());

            DiContext.Container.Register(typeof(IUiController),typeof(MvpFramework.WinForms.UiController));

            //            DiContext.ScanAssembly();  // same as passing this.GetType().Assembly

            //            DiContext.Container.Register(typeof(EditContactView));
            //            DiContext.Container.Options.DefaultScopedLifestyle = ;
            //            DiContext.Container.Register<EditContactView>(Lifestyle.Scoped);

            /*
            //TODO: Automatic registration:
            DiContext.Container.Register(typeof(IEditContactPresenter), typeof(EditContactPresenter));
            //      DiContext.Container.Register(typeof(IEditContactView), typeof(EditContactView));
            DiContext.Container.Register(typeof(IEditContactView), () => new EditContactView());

            //            DiContext.Container.Register(typeof(MainForm));


            //            DiContext.RegisterTypes(typeof(IView));

            DiContext.Container.Register(typeof(IPresenterFactory<IEditContactPresenter, Contact>),
                    () => new PresenterFactory<IEditContactPresenter, Contact>(Resolver, DiContext)
                );
                */

            /* Could also be registered as (less efficient):
            DiContext.Container.RegisterSingleton(typeof(IPresenterFactory<IEditContactPresenter, Contact>),
                new PresenterFactory<IEditContactPresenter,Contact>(Resolver, DiContext)
            );
            */

            DiContext.Container.RegisterSingleton(typeof(IControlBinderFactory), new ControlBinderFactory());


            // Get configuration settings from database (could be a few objects, each for a category of settings)
            // and/or an IConfigValueProvider for the Registry, command line, configuration file, etc.,
            // and add to ProviderChain.

            DiContext.ProviderChain.RegisterProvider(new CommandLineConfigProvider());

//            DiContext.Container.Verify();
        }

        protected SiExtendedDiContext DiContext = new SiExtendedDiContext();
        protected MvpResolver Resolver;
    }
}
