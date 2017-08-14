using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework
{
    /// <summary>
    /// Interface for a factory that creates a Presenter.
    /// Can be used for automatic factory creation on dependency injection.
    /// </summary>
    /// <typeparam name="TPresenter">Type of the presenter created by the factory.</typeparam>
    public interface IPresenterFactory<TPresenter>
        where TPresenter : IPresenter
    {
        /// <summary>
        /// Create the Presenter.
        /// </summary>
        /// <returns></returns>
        TPresenter Create();
    }

    #region Generic versions
    // Generic types for different parameter counts.

    /// <summary>
    /// Interface for a factory that creates a Presenter.
    /// Can be used for automatic factory creation on dependency injection.
    /// </summary>
    /// <typeparam name="TPresenter">Type of the presenter created by the factory.</typeparam>
    /// <typeparam name="TParam">Type of the parameter to the Create method.</typeparam>
    public interface IPresenterFactory<TPresenter, TParam>
        where TPresenter : IPresenter
    {
        /// <summary>
        /// Create the Presenter.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        TPresenter Create(TParam param);
    }

    /// <summary>
    /// Interface for a factory that creates a Presenter.
    /// Can be used for automatic factory creation on dependency injection.
    /// </summary>
    /// <typeparam name="TPresenter">Type of the presenter created by the factory.</typeparam>
    /// <typeparam name="TParam1">The type of the first parameter to the Create method.</typeparam>
    /// <typeparam name="TParam2">The type of the second parameter to the Create method.</typeparam>
    public interface IPresenterFactory<TPresenter, TParam1, TParam2>
        where TPresenter : IPresenter
    {
        TPresenter Create(TParam1 param1, TParam2 param2);
    }

    /// <summary>
    /// Interface for a factory that creates a Presenter.
    /// Can be used for automatic factory creation on dependency injection.
    /// </summary>
    /// <typeparam name="TPresenter">Type of the presenter created by the factory.</typeparam>
    /// <typeparam name="TParam1">The type of the first parameter to the Create method.</typeparam>
    /// <typeparam name="TParam2">The type of the second parameter to the Create method.</typeparam>
    /// <typeparam name="TParam3">The type of the third parameter to the Create method.</typeparam>
    public interface IPresenterFactory<TPresenter, TParam1, TParam2, TParam3>
        where TPresenter : IPresenter
    {
        TPresenter Create(TParam1 param1, TParam2 param2, TParam3 param3);
    }

    /// <summary>
    /// Interface for a factory that creates a Presenter.
    /// Can be used for automatic factory creation on dependency injection.
    /// </summary>
    /// <typeparam name="TPresenter">Type of the presenter created by the factory.</typeparam>
    /// <typeparam name="TParam1">The type of the first parameter to the Create method.</typeparam>
    /// <typeparam name="TParam2">The type of the second parameter to the Create method.</typeparam>
    /// <typeparam name="TParam3">The type of the third parameter to the Create method.</typeparam>
    /// <typeparam name="TParam4">The type of the fourth parameter to the Create method.</typeparam>
    public interface IPresenterFactory<TPresenter, TParam1, TParam2, TParam3, TParam4>
        where TPresenter : IPresenter
    {
        TPresenter Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4);
    }

    /// <summary>
    /// Interface for a factory that creates a Presenter.
    /// Can be used for automatic factory creation on dependency injection.
    /// </summary>
    /// <typeparam name="TPresenter">Type of the presenter created by the factory.</typeparam>
    /// <typeparam name="TParam1">The type of the first parameter to the Create method.</typeparam>
    /// <typeparam name="TParam2">The type of the second parameter to the Create method.</typeparam>
    /// <typeparam name="TParam3">The type of the third parameter to the Create method.</typeparam>
    /// <typeparam name="TParam4">The type of the fourth parameter to the Create method.</typeparam>
    /// <typeparam name="TParam5">The type of the fifth parameter to the Create method.</typeparam>
    public interface IPresenterFactory<TPresenter, TParam1, TParam2, TParam3, TParam4, TParam5>
        where TPresenter : IPresenter
    {
        TPresenter Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5);
    }

    #endregion


    /// <summary>
    /// For Presenter Factories nested in another Presenter with a corresponding nested view (nested in the outer Presenter's view).
    /// This interface allows providing the nested view to the presenter being creating the nested Presenter.
    /// </summary>
    public interface INestedPresenterFactory
    {
        /// <summary>
        /// The View to be used for any presenter created by this factory.
        /// null to have a View created on each Create call.
        /// </summary>
        IView View { get; set; }

        /// <summary>
        /// The view in which the view of the created presenter is to be created.
        /// </summary>
        IView ContainingView { get; set; }

        /// <summary>
        /// ID of the nested view within the containing one.
        /// </summary>
        string NestedViewId { get; set; }
    }

}
