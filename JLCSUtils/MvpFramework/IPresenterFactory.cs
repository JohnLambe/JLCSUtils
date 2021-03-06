﻿using System;
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
    /// <typeparam name="TParam6">The type of the sixth parameter to the Create method.</typeparam>
    public interface IPresenterFactory<TPresenter, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
        where TPresenter : IPresenter
    {
        TPresenter Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6);
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
    /// <typeparam name="TParam6">The type of the sixth parameter to the Create method.</typeparam>
    /// <typeparam name="TParam7">The type of the seventh parameter to the Create method.</typeparam>
    public interface IPresenterFactory<TPresenter, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>
        where TPresenter : IPresenter
    {
        TPresenter Create(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7);
    }

    #endregion


    /// <summary>
    /// For Presenter Factories nested in another Presenter with a corresponding nested view (nested in the outer Presenter's view).
    /// This interface allows providing the nested view to the presenter being creating the nested Presenter.
    /// </summary>
    /// <seealso cref="MvpNestedAttribute"/>
    public interface INestedPresenterFactory
    {
        /// <summary>
        /// The View to be used for any presenter created by this factory.
        /// null to have a View created on each Create call.
        /// </summary>
        IView View { get; set; }

        /// <summary>
        /// The view in which the view of the created presenter is to be created.
        /// null if the created view is not to be nested.
        /// </summary>
        IView ContainingView { get; set; }

        /// <summary>
        /// ID of the nested view within the containing one.
        /// null if the created view is not to be nested.
        /// </summary>
        string NestedViewId { get; set; }
    }


    /// <summary>
    /// For Presenter Factories that support creating child contexts for each presenter.
    /// This interface allows configuring whether the factory should create new a context per presenter.
    /// </summary>
    /// <seealso cref="MvpNestedAttribute"/>
    public interface ISharedContextPresenterFactory
    {
        /// <summary>
        /// true to create a new dependency injection context (a child of the given one) for each presenter created.
        /// </summary>
        bool? UseChildContext { get; set; }
    }

}
