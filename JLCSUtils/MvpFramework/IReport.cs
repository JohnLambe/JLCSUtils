using JohnLambe.Util.Collections;
using JohnLambe.Util.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework
{
    // Desktop-based interface:
    // TODO: Interface more suitable for web.
    // 
    
    /// <summary>
    /// Interface for a report or other tabular view of data (e.g. search results, any grid of data).
    /// </summary>
    /// <typeparam name="TParameters">The type of the parameters object.</typeparam>
    public interface IReport<TParameters>
    {
        /// <summary>
        /// The capabilities of the implementation.
        /// </summary>
        ReportCapabilities Capabilities { get; }

        /// <summary>
        /// Show the output.
        /// This may be a modal or non-modal dialog.
        /// </summary>
        /// <returns>
        /// Implementation-dependent.
        /// null if not supported.
        /// </returns>
        object Show();

        /// <summary>
        /// Print the report/data, if supported.
        /// </summary>
        /// <returns>The modal result of the print dialog. See <see cref="Dialog.MessageDialogResponse"/>.
        /// null if Printing is not supported.
        /// </returns>
        object Print(IPrintSettings settings = null);

        /// <summary>
        /// Show a modal user interface to enter the parameters.
        /// </summary>
        /// <returns>The modal result. See <see cref="Dialog.MessageDialogResponse"/>.
        /// null if not supported.
        /// </returns>
        object ShowParameters();

        /// <summary>
        /// Returns a bound view for displaying/entering the parameters.
        /// This could be placed in a user interface next to the view of the data (results).
        /// </summary>
        /// <returns></returns>
        IParametersView GetParametersView();

        /// <summary>
        /// The parameters affecting the report.
        /// These may affect filtering, sorting and/or layout.
        /// Changing this does NOT refresh the data implicitly. (It can be refreshed explicitly by calling <see cref="Refresh"/>).
        /// <para>Can be null if no parameters are supported.</para>
        /// </summary>
        [Nullable]
        TParameters Parameters { get; set; }

        /// <summary>
        /// The data of the report (the output).
        /// This populates the data (typically by running an underlying query) if it is not already populated.
        /// </summary>
        IQueryable MainData { get; set; }

        /// <summary>
        /// For views/reports with more complex data, such as a set of grids, or other values.
        /// </summary>
        [Nullable]
        ISimpleLookup<string, object> Data { get; set;}

        /// <summary>
        /// Refresh the data.
        /// This typically re-runs the underlying query.
        /// <para>This affects <see cref="MainData"/> and <see cref="Data"/>.</para>
        /// </summary>
        void Refresh();

        // bool IsPopulated ?
    }

    public interface IReport : IReport<object>
    {
    }

    [Flags]
    public enum ReportCapabilities
    {
        CanPrint = 1,
        CanShow = 2,
        CanShowParameters = 4,
        GetParametersView = 8,
        /// <summary>
        /// Set if any parameters are supported.
        /// </summary>
        HasParameters = 16,
    }

    /// <summary>
    /// Settings for printing a document (mainly the settings typically show in a print dialog).
    /// </summary>
    public interface IPrintSettings
    {
        bool ShowDialog { get; set; }

        /// <summary>
        /// The number of copies to print.
        /// </summary>
        int Copies { get; set; }  // Default: 1

        /// <summary>
        /// A list of page ranges to print.
        /// "" for all.
        /// <para>Format (BNF):<br/>
        /// Range = [1*DIGIT] ["-" [1*DIGIT]]<br/>
        /// Pages = Range *("," Range)
        /// </para>
        /// </summary>
        string Pages { get; set; }  // Default: ""

        /// <summary>
        /// Printer name (the Operating System's name for the printer).
        /// </summary>
        string Printer { get; set; }

        // Even / Odd / All

        //TODO
    }

    public interface IParametersView : IView
    {
        /// <summary>
        /// Fired when the data affected by these parameters should be refreshed.
        /// </summary>
        event EventHandler OnRefresh;
    }
}
