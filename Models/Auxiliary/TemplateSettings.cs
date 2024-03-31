using System.Collections.Generic;

namespace Incubator_2.Models.Auxiliary
{
    public struct TransferMatch
    {
        public int From;
        public int To;
    }
    public struct Transfer
    {
        public string name;
        public int template;
        public List<TransferMatch> content;
    }


    public struct Trigger
    {
        public string name;
    }

    public struct TemplateSettings
    {
        public List<Transfer> transfers;
        public List<Trigger> triggers;
    }
}
