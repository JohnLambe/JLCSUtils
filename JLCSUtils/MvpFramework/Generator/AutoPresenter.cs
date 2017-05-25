using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiExtension.Attributes;
using MvpFramework.Binding;

namespace MvpFramework.Generator
{

    public interface IAutoPresenter : IPresenter
    {
    }

    public interface IAutoView : IWindowView
    {
    }

    //[Presenter(Interface = typeof(IAutoPresenter))]
    public class AutoPresenterBase<TView, TModel> : WindowPresenterBase<TView, TModel>, IAutoPresenter
        where TView : IAutoView
    {
        public AutoPresenterBase(TView view,
            [MvpParam] TModel model = default(TModel),
            [Inject] IControlBinderFactory binderFactory = null) : base(view, model, binderFactory)
        {
        }
    }

    [Presenter(Interface = typeof(IAutoPresenter))]
    public class AutoPresenter : AutoPresenterBase<IAutoView, object>
    {
        public AutoPresenter(IAutoView view,
            [MvpParam] object model = null,
            [Inject] IControlBinderFactory binderFactory = null) : base(view, model, binderFactory)
        {
        }
    }
}
