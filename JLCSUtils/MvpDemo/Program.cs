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
using MvpFramework.WinForms;
using MvpFramework.Dialog;
using JohnLambe.Util.Misc;

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
//            SetUpExceptionHandler();

            //throw new Exception("Test2");

            new Program().Run();
        }

        private static void SetUpExceptionHandler()
        {
            // Add the event handler for handling UI thread exceptions to the event.
            Application.ThreadException += Application_ThreadException;

            // Set the unhandled exception mode to force all Windows Forms errors
            // to go through our handler.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Add the event handler for handling non-UI thread exceptions to the event. 
//            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString() + "\n" + sender.ToString());

        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message + "\n" + sender.ToString());
        }

        public void Run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);   // so that the ApplicationExceptionHandler can be set up

            SetupDi();

            _exceptionHandler = new ApplicationExceptionHandler(DiContext.GetInstance<IMessageDialogService>());

            var mainForm = DiContext.GetInstance<MainForm>();

            Application.Run(mainForm);
        }

        protected void SetupDi()
        {
            var assemblies = AssemblyUtil.GetReferencedAssemblies(Assembly.GetEntryAssembly(),true);
            
            Resolver = new DiMvpResolver(DiContext);
            DiContext.Container.RegisterSingleton<IDiResolver>(DiContext);
            DiContext.Container.RegisterSingleton(Resolver);
            //                typeof(MvpResolver),Resolver);

            //            DiContext.Container.Register(typeof(IResolverExtension), typeof(NullUiManager));
            DiContext.Container.Register(typeof(IResolverExtension), typeof(ResolverExtension));

            //            DiContext.Container.Register(typeof(IMessageDialogService),typeof(BasicMessageDialogService));
            DiContext.Container.Register(typeof(IMessageDialogService), typeof(MessageDialogService));
//            DiContext.Container.RegisterSingleton<IMessageDialogService>(new MessageDialogService());

            new RegistrationHelper(Resolver, DiContext).ScanAssemblies(assemblies.ToArray()); // Assembly.GetExecutingAssembly());

            DiContext.Container.Register(typeof(IUiController),typeof(MvpFramework.WinForms.UiController));

            DiContext.Container.Register(typeof(IIconRepository<string,object>), typeof(NullIconRepository<string,object>));


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

            DiContext.Container.RegisterSingleton(typeof(IControlBinderFactory), new ControlBinderFactory(DiContext));


            // Get configuration settings from database (could be a few objects, each for a category of settings)
            // and/or an IConfigValueProvider for the Registry, command line, configuration file, etc.,
            // and add to ProviderChain.

            DiContext.ProviderChain.RegisterProvider(new CommandLineConfigProvider());

            //            DiContext.Container.Verify();

        }

        protected SiExtendedDiContext DiContext = new SiExtendedDiContext();
        protected MvpResolver Resolver;
        protected ApplicationExceptionHandler _exceptionHandler;
    }
}
