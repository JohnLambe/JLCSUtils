using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using JohnLambe.Util.Validation;
using MvpFramework.Binding;

namespace MvpDemo.Model
{
    [GroupDefinition(Id = "Additional", DisplayName = "Additional Details")]
    public class Contact  // could also be called ContactModel
    {
        [MvpDisplay(IsVisible = false)]
        public int Id { get; set; }

        [MaxLength(20)]   // sets maximum length of edit box control
        [Required]        // Not handled yet
        public string Name { get; set; }

        [StringValidation(Capitalisation = JohnLambe.Util.Text.LetterCapitalizationOption.TitleCase)]
        [InvalidValue("X")]
        [InvalidValue("*")]
        [Display(Order = 2000)]
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }
        protected string _address;

        [PhoneNumberValidation]
        [Display(Order = 1000)]
        public string PhoneNumber { get; set; }

        [PhoneNumberValidation]
        [Display(Order = 1000)]
        public string PhoneNumber2 { get; set; }

        [PhoneNumberValidation]
        [Display(Order = 1010)]
        public string FaxNumber { get; set; }

        [Display(GroupName = "Additional")]
        public string Comment { get; set; }

        [Display(GroupName = "Additional", Name = "Other details")]
        public string Other { get; set; }


        [Display(AutoGenerateField = false)]
        public virtual string EntityDescription => "Contact";
    }

}
