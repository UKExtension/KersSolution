using System;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Abstract;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class Message : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Subject {get; set;}
        public string Body {get;set;}
        public KersUser From {get; set;}
        public KersUser To {get; set;}
        public string FromEmail {get; set;} // In case From is not a KersUser. For Example, messages that should be sent on behalf of the system.
        public string ToEmail {get;set;} // In case To is not a KersUser
        public int Type {get;set;} // To handle cases when messages are not delivered by email
        public DateTimeOffset ScheduledFor {get;set;} // Message should be sent after this time
        public Boolean IsItSent {get;set;}
        public string Status {get;set;} // Eventual Status Message after it was sent https://github.com/jstedfast/MailKit/issues/126
        public DateTimeOffset SentAt {get;set;}
        public DateTimeOffset Created {get; set;}
        public DateTimeOffset Updated {get; set;}
    }
}