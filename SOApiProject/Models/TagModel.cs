namespace SOApiProject.Models;
public class TagModel
{
    public bool HasSynonyms { get; set; }
    public bool IsModeratorOnly { get; set; }
    public bool IsRequired { get; set; }
    public int Count { get; set; }
    public string Name { get; set; }
}
public class Response
{
    public List<TagModel> Items { get; set; }
}