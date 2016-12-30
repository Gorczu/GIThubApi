using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GitHubTask
{
    public class UserNoRepositories
    {
        [XmlAttribute]
        public string Login { get; set; }

        [XmlAttribute]
        public int NoOfRepositories { get; set; }
    }
}
