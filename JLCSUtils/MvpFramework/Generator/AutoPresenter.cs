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

    //[Presenter(Interface = typeof(IAutoPresenter))]
    public class AutoPresenter<TView, TModel> : PresenterBase<TView, TModel>, IAutoPresenter
        where TView : IView
    {
        public AutoPresenter(TView view,
            [MvpParam] TModel model = default(TModel),
            [Inject] IControlBinderFactory binderFactory = null) : base(view, model, binderFactory)
        {
        }
    }
}
