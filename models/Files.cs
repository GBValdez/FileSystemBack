using project.utils;

namespace back.models
{
    public class Files : CommonsModel<long>
    {
        public string Name { get; set; }
        public string OriginalName { get; set; }
    }
}