using System.Windows;

namespace Ovotan.Windows.Controls
{
    /// <summary>
    /// Менеджер схем стилизации.
    /// </summary>
    public class SchemaManager : ResourceDictionary
    {
        /// <summary>
        /// Словарь загруженных схем.
        /// </summary>
        static Dictionary<string, string> _loadedSchemas;
        /// <summary>
        /// Ссылка на действие добавления новой схемы.
        /// </summary>
        static Action<string, string> _callback;

        /// <summary>
        /// get,set - Название схемы по умолчанию.
        /// </summary>
        public string DefaultSchemaName { get; set; }

        /// <summary>
        /// Конструктор.
        /// </summary>
        static SchemaManager()
        {
            _loadedSchemas = new Dictionary<string, string>();
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        public SchemaManager()
        {
            _callback = _mergeResource;
            if(!Application.Current.Resources.MergedDictionaries.Contains(this))
            {
                Application.Current.Resources.MergedDictionaries.Add(this);
            }
        }

        /// <summary>
        /// Добавление нового ресурса схемы.
        /// </summary>
        /// <param name="source">Uri ресурса.</param>
        /// <param name="schemaNamePattern">
        /// Шаблон части Uri который нужно заменить на текущую схему.
        /// Например: 
        /// source = pack://application:,,,/Ovotan.Windows.Controls.EndPointManagement;component/Schemas/{Schemas}/DialogResource.xaml
        /// schemaNamePattern = {Schemas}
        /// получим - source = pack://application:,,,/Ovotan.Windows.Controls.EndPointManagement;component/Schemas/Blue/DialogResource.xaml"
        /// </param>
        public static void AddResource(string source, string schemaNamePattern)
        {
            lock (_loadedSchemas)
            {
                _callback(source, schemaNamePattern);
            }
        }

        /// <summary>
        /// Смена текущей схемы.
        /// </summary>
        /// <param name="schema">Название новой схемы</param>
        public void ChangeSchema(string schema)
        {
            lock (_loadedSchemas)
            {
                DefaultSchemaName = schema;
                MergedDictionaries.Clear();
                foreach (var kvp in _loadedSchemas)
                {

                    var source = kvp.Key.Replace(kvp.Value, schema);
                    var resource = new ResourceDictionary() { Source = new Uri(source) };
                    MergedDictionaries.Add(resource);
                }
            }
        }

        /// <summary>
        /// Добавление нового ресурса схемы.
        /// </summary>
        /// <param name="source">Uri ресурса.</param>
        /// <param name="schemaNamePattern">
        /// Шаблон части Uri который нужно заменить на текущую схему.
        /// Например: 
        /// source = pack://application:,,,/Ovotan.Windows.Controls.EndPointManagement;component/Schemas/{Schemas}/DialogResource.xaml
        /// schemaNamePattern = {Schemas}
        /// получим - source = pack://application:,,,/Ovotan.Windows.Controls.EndPointManagement;component/Schemas/Blue/DialogResource.xaml"
        /// </param>
        void _mergeResource(string source, string schemaNamePattern)
        {
            if (!_loadedSchemas.ContainsKey(source))
            {
                _loadedSchemas.Add(source, schemaNamePattern);
                if (!string.IsNullOrEmpty(schemaNamePattern))
                {
                    source = source.Replace(schemaNamePattern, DefaultSchemaName);
                }
                var resource = new ResourceDictionary() { Source = new Uri(source) };
                MergedDictionaries.Add(resource);
            }
        }
    }
}
