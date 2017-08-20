using DiExtension.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework
{
    /// <summary>
    /// Base class for injection attributes of the MVP framework.
    /// </summary>
    public abstract class MvpInjectAttribute : InjectAttribute
    {        
    }

    /// <summary>
    /// Flags a parameter to be injected by the MVP framework.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class MvpParamAttribute : MvpInjectAttribute
    {
    }

    /// <summary>
    /// Flags that a Presenter being injected should get a View nested in the View of the Presenter
    /// that it is being injected into.
    /// Currently supported only on constructor injection.
    /// </summary>
    /// <example>
    /// <code>
    /// // Presenter constructor:
    ///    public TestLayoutPresenter(ITestLayoutView view,
    ///        [MvpParam] TestHViewModel model,
    ///        [Inject] IControlBinderFactory binderFactory,
    ///        [MvpNested("Contact")] IPresenterFactory&lt;IEditContactPresenter, Contact&gt; editContactFactory,  // inject the nested presenter factory
    ///    ) : base(view, model, binderFactory)
    ///    {
    ///        Contact = editContactFactory.Create(model.Contact);     // create the nested presenter, bound to a view inside the view of this presenter
    ///    }
    ///         
    ///    public IEditContactPresenter Contact { get; protected set; }       // Nested presenter.
    /// </code>
    /// 
    /// <para>
    /// The view would have a placeholder for the nested view, in a control implementing <see cref="INestedView"/>
    /// (such as <see cref="MvpFramework.WinForms.Controls.MvpNestedViewPlaceholder"/> for WinForms), with a <see cref="INestedView.ViewId"/> of "Contact".
    /// </para>
    /// <para>
    /// The view of IEditContactPresenter must implement <see cref="INestableView"/>.
    /// </para>
    /// <para>
    /// See MvpDemo.Heirarchical.TestLayoutPresenter in the MvpDemo project.
    /// </para>
    /// </example>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class MvpNestedAttribute : MvpInjectAttribute
    {
        /// <summary>
        /// </summary>
        /// <param name="nestedViewId"><see cref="NestedViewId"/></param>
        public MvpNestedAttribute(string nestedViewId = null)
        {
            this.NestedViewId = nestedViewId;
        }

        /// <summary>
        /// <inheritdoc cref="INestedPresenterFactory.NestedViewId"/>
        /// </summary>
        public string NestedViewId { get; set; }
    }
}
