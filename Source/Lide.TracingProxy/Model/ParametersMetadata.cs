namespace Lide.TracingProxy.Model
{
    public class ParametersMetadata
    {
        private readonly object[] _originalParameters;
        private readonly object[] _editedParameters;

        public ParametersMetadata(ParametersMetadataVolatile parametersMetadataVolatile)
        {
            _originalParameters = parametersMetadataVolatile.GetOriginalParameters();
            _editedParameters = parametersMetadataVolatile.GetEditedParameters();
        }

        public object[] GetOriginalParameters() => _originalParameters;
        public object[] GetEditedParameters() => _editedParameters;
    }
}