namespace PleasantlyGames.RPG.Runtime.Save.DataTransformers
{
    public abstract class DataTransformerDecorator : IDataTransformer
    {
        private readonly IDataTransformer _wrappedTransformer;

        protected DataTransformerDecorator(IDataTransformer wrappedTransformer) => 
            _wrappedTransformer = wrappedTransformer;

        public virtual string Transform(string data) => 
            _wrappedTransformer.Transform(data);

        public virtual string Reverse(string data) => 
            _wrappedTransformer.Reverse(data);
    }
}