using JohnLambe.Util;
using JohnLambe.Util.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvpFramework.Dialog
{
    /// <summary>
    /// The model of <see cref="IMessageDialogView"/>.
    /// </summary>
    public class MessageDialogViewModel
    {
        /// <summary>
        /// Model of the current dialog.
        /// </summary>
        public virtual IMessageDialogModel Dialog { get; set; }

        /// <summary>
        /// Window title (for the current dialog).
        /// </summary>
        public virtual string Title => Dialog?.Title;

        public virtual string DetailMessage =>
            Dialog?.Exception == null ? null
            : (Dialog?.Message ?? "") + "\r\n\r\n"
            //+ (Dialog?.Exception?.ToString());
            + (ExceptionUtil.ExtractException(Dialog?.Exception)?.Message)
            .ReplaceLineSeparator(LineSeparator);

        public const string LineSeparator = Consts.LineSeparator;
    }
}
