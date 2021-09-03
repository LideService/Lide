namespace Lide.TracingProxy.Model
{
    public class ParametersMetadataVolatile
    {
        private readonly object[] _originalParameters;
        private object[] _editedParameters;
        private bool _areParametersEdited;

        public ParametersMetadataVolatile(object[] originalParameters)
        {
            _originalParameters = originalParameters;
        }

        public ParametersMetadataVolatile(object[] originalParameters, object[] editedParameters)
        {
            _originalParameters = originalParameters;
            SetParameters(editedParameters);
        }

        public object[] GetOriginalParameters() => _originalParameters;
        public object[] GetEditedParameters() => _areParametersEdited ? _editedParameters : _originalParameters;
        public bool AreParametersEdited() => _areParametersEdited;
        public void SetParameters(object[] editedParameters)
        {
            if (object.ReferenceEquals(_originalParameters, editedParameters))
            {
                return;
            }

            _editedParameters = editedParameters;
            _areParametersEdited = true;
        }

        public ParametersMetadataVolatile Clone()
        {
            return new ParametersMetadataVolatile(_originalParameters)
            {
                _editedParameters = _editedParameters,
                _areParametersEdited = _areParametersEdited,
            };
        }
    }
}