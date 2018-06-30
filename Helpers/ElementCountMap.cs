using System;
using System.Collections.Generic;

namespace AzureAppServiceMigrationAssistant.Helpers
{
    public class ElementCountMap
    {
        private Dictionary<string, int> _elementCollection;

        public ElementCountMap()
        {
            _elementCollection = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        public void Add(string element)
        {
            lock (_elementCollection)
            {
                int value;
                if (_elementCollection.TryGetValue(element, out value))
                {
                    _elementCollection[element] = value + 1;
                }
                else
                {
                    _elementCollection[element] = 1;
                }
            }
        }

        public int this[string name]
        {
            get
            {
                return _elementCollection[name];
            }
        }

        public int Length
        {
            get
            {
                return _elementCollection.Count;
            }
        }
    }
}
