using System;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace NXA.SC.Caas.Models {
    public sealed class LoginInfo
    {
        [Required]
        [DataMember]
        public string Login { get; set; } = String.Empty;
        [Required]
        [DataMember]
        public string Password { get; set; } = String.Empty;
        public bool Valid 
        { 
            get
            {
                //TODO: validate user
                return true;
            }
        }
    }
}
