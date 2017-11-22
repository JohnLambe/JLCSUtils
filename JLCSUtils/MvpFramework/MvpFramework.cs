using MvpFramework.Dialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework
{
    /// <summary>
    /// Interface to the framework - groups things to be injected into many presenters/views,
    /// and enables adding items to be injected into base classes without requiring changes to the subclasses.
    /// </summary>
    public interface IMvpFrameworkDetails
    {
        IMessageDialogService MessageDialogService { get; }
        // IControlBinderFactory ?
        // Creation of ViewBinder ?
        // IUiController ?
    }

    // Separate interfaces for Views and Presenters derived from this?


    public class MvpFrameworkDetails : IMvpFrameworkDetails
    {
        public MvpFrameworkDetails(IMessageDialogService messageDialogService)
        {
            this.MessageDialogService = messageDialogService;
        }

        public IMessageDialogService MessageDialogService { get; }
    }
}
