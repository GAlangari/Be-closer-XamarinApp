using BeCloser.Models;
using Xamarin.Forms;

namespace BeCloser.Views.Templates
{
    public class ChatDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FromTemplate { get; set; }
        public DataTemplate ToTemplate { get; set; }

        public ChatDataTemplateSelector()
        {
            FromTemplate = new DataTemplate(typeof(IncomingMessage));
            ToTemplate = new DataTemplate(typeof(OutgoingMessage));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item != null)
            {
                return ((Message)item).to.Equals(App.TeacherId) ? FromTemplate : ToTemplate;
            }
            else
            {
                return ToTemplate;
            }
        }
    }
}
