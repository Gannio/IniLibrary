using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace IniLibrary
{
    
    /// <summary>
    /// The generalized INI library class.
    /// </summary>
    public class INI : IList<INI.Section>
    {

        #region Structures

        /// <summary>
        /// Contains a list of Keys.
        /// </summary>
        public class Section : IList<Key>
        {

            #region Section Core

            /// <summary>
            /// The name of this Section.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// This is the list of keys associated with the Section.
            /// </summary>
            public List<Key> Keys { get; set; }

            /// <summary>
            /// Gets or sets a key whose name is the given value.
            /// </summary>
            /// <param name="keyName">The name of the key to return.</param>
            /// <returns>A Key Class which contains a value.</returns>
            public Key this[string keyName]
            {
                get
                {
                    if(!Keys.Exists(k => k.Name.Equals(keyName)))
                        Keys.Add(new Key {Name = keyName, Value = string.Empty});
                    return Keys.First(k => k.Name.Equals(keyName));
                }
                set
                {
                    if(Keys.Exists(k => k.Name.Equals(keyName)))
                    {
                        for (var i = 0; i < Keys.Count; i++)
                        {
                            if (!Keys[i].Name.Equals(keyName)) continue;
                            Keys[i] = value;
                            return;
                        }
                    }
                    else
                    {
                        Keys.Add(value);
                    }
                }
            }

            /// <summary>
            /// Gets or sets a key at the given index.
            /// </summary>
            /// <param name="index">The index of the key to get or set.</param>
            /// <returns>A Key Class which contains a value.</returns>
            /// <exception cref="IndexOutOfRangeException">The given index is invalid.</exception>
            public Key this[int index]
            {
                get { return Keys[index]; }
                set { Keys[index] = value; }
            }

            /// <summary>
            /// Returns a string representation of this section.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return string.Format("Section: {0}{1}Key Count: {2}", Name, Environment.NewLine, Keys.Count);
            }

            #endregion

            #region IList Implementation

            /// <summary>
            /// Returns the index of the specified key.
            /// </summary>
            /// <param name="item">The item to look up.</param>
            /// <returns></returns>
            public int IndexOf(Key item)
            {
                if (Keys.Contains(item))
                    return Keys.IndexOf(item);
                return -1;
            }

            /// <summary>
            /// Inserts a key at the specified index.
            /// </summary>
            /// <param name="index">The index to insert the key at.</param>
            /// <param name="item">The key to insert.</param>
            public void Insert(int index, Key item)
            {
                if (index > -1 && index <= Keys.Count() - 1)
                    Keys.Insert(index, item);
            }

            /// <summary>
            /// Removes a key at the specified index.
            /// </summary>
            /// <param name="index">The index to remove the key at.</param>
            public void RemoveAt(int index)
            {
                if (index > -1 && index <= Keys.Count() - 1)
                    Keys.RemoveAt(index);
            }

            /// <summary>
            /// Adds a key to the collection.
            /// </summary>
            /// <param name="item">The key to add.</param>
            public void Add(Key item)
            {
                if (!Keys.Contains(item))
                    Keys.Add(item);
            }

            /// <summary>
            /// Clears the keys from the section.
            /// </summary>
            public void Clear()
            {
                Keys.Clear();
            }

            /// <summary>
            /// Checks to see if a key exists.
            /// </summary>
            /// <param name="item">The key to check for.</param>
            /// <returns></returns>
            public bool Contains(Key item)
            {
                return Keys.Contains(item);
            }

            /// <summary>
            /// Copies the keys to an array, starting that the appropriate index.
            /// </summary>
            /// <param name="array">The array to copy to.</param>
            /// <param name="arrayIndex">The start index.</param>
            public void CopyTo(Key[] array, int arrayIndex)
            {
                Keys.CopyTo(array, arrayIndex);
            }

            /// <summary>
            /// Returns how many items are in the key collection.
            /// </summary>
            public int Count
            {
                get { return Keys.Count(); }
            }

            /// <summary>
            /// Always returns false.
            /// </summary>
            public bool IsReadOnly
            {
                get { return false; }
            }

            /// <summary>
            /// Removes a key from the collection.
            /// </summary>
            /// <param name="item">The key to remove.</param>
            /// <returns></returns>
            public bool Remove(Key item)
            {
                return Keys.Remove(item);
            }

            /// <summary>
            /// Returns the appropriate enumerator.
            /// </summary>
            /// <returns></returns>
            public IEnumerator<Key> GetEnumerator()
            {
                return Keys.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return Keys.GetEnumerator();
            }

            #endregion

        }

        /// <summary>
        /// Contains the name of the key as well as its' associated value.
        /// </summary>
        public class Key
        {

            /// <summary>
            /// This is the name of the key.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// The associated value of the key.
            /// </summary>
            public object Value { get; set; }

            /// <summary>
            /// A special comment for this particular key.
            /// </summary>
            public string Comment { get; set; }

            /// <summary>
            /// Returns the string representation
            /// of the key.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                var format = String.IsNullOrEmpty(Comment.Trim()) ? "{0}: {1}" : "{0}: {1} ;{2}";
                return string.Format(format, Name, Value, Comment);
            }

        }

        #endregion

        #region Variables

        /// <summary>
        /// A list of all the sections in the INI file.
        /// </summary>
        private readonly List<Section> _sections;

        #endregion

        #region Enums

        /// <summary>
        /// The save type of the INI class.
        /// </summary>
        public enum Type
        {

            /// <summary>
            /// Save the class as a normal INI file.
            /// </summary>
            INI,

            /// <summary>
            /// Save the class as an XML file.
            /// </summary>
            XML
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns a list of all the sections in the INI file.
        /// </summary>
        public IEnumerable<string> Sections
        {
            get
            {
                return this.Select(item => item.Name);
            }
        }

        /// <summary>
        /// Returns how many sections are in the INI file.
        /// </summary>
        public int Count { get { return this.Count(); } }

        /// <summary>
        /// This routine always will return false.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

        #region Initialization
        
        /// <summary>
        /// Initializes the INI class with a list of sections.
        /// </summary>
        /// <param name="sections">The sections to initialize the INI class with.</param>
        public INI(IEnumerable<Section> sections) : this()
        {
            AddRange(sections);
        }

        /// <summary>
        /// Initializes the INI class with a blank list of sections.
        /// </summary>
        /// <param name="sectionNames">The names of the sections.</param>
        public INI(IEnumerable<String> sectionNames) : this()
        {
            AddRange(sectionNames);
        }

        /// <summary>
        /// Initializes the INI class with a file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="filetype">The type of file to load.</param>
        public INI(string path, Type filetype) : this()
        {
            Load(path, filetype);
        }

        /// <summary>
        /// Initialize the INI class.
        /// </summary>
        public INI()
        {
            _sections = new List<Section>();
        }

        #endregion

        #region Indexers

        /// <summary>
        /// Retrieves or sets a section based on the index.
        /// </summary>
        /// <param name="index">The index of the section.</param>
        /// <returns>A section containing a list of different keys.</returns>
        /// <exception cref="IndexOutOfRangeException">The given index is invalid.</exception>
        public Section this[int index]
        {
            get { return _sections[index]; }
            set { _sections[index] = value; }
        }

        /// <summary>
        /// Retrieves or sets a section based on the name.
        /// </summary>
        /// <param name="name">The name of the section.</param>
        /// <returns>A section containing a list of different keys.</returns>
        public Section this[string name]
        {
            get 
            {
                if (!Contains(name)) Add(name);
                return this.First(s => s.Name.Equals(name));
            }
            set 
            {
                for (var i = 0; i < this.Count(); i++)
                {
                    if (!this[i].Name.Equals(name)) continue;
                    this[i] = value; return;
                }
            }
        }

        #endregion

        #region Add

        /// <summary>
        /// Adds a new section to the INI file.
        /// </summary>
        /// <param name="section">The section structure to add.</param>
        public void Add(Section section)
        {
            if (!Contains(section.Name)) 
                _sections.Add(section); 
            else
                foreach (var key in section.Keys.Where(key => !Contains(section.Name, key.Name)))
                    Add(section.Name, key.Name, key.Value);
        }
        
        /// <summary>
        /// Adds a new section to the INI file.
        /// </summary>
        /// <param name="sectionName">The name of the section to add.</param>
        public void Add(string sectionName)
        {
            if (!Contains(sectionName)) Add(new Section { Keys = new List<Key>(), Name = sectionName });
        }

        /// <summary>
        /// Adds a new key to a section. If the section does not exist, it will be created.
        /// </summary>
        /// <param name="sectionName">The name of the section.</param>
        /// <param name="keyName">The name of the key.</param>
        /// <param name="keyValue">The value of the key.</param>
        /// <exception cref="ArgumentException" />
        public void Add<T>(string sectionName, string keyName, T keyValue)
        {
            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentException("The section name cannot be empty!");
            if (string.IsNullOrEmpty(keyName))
                throw new ArgumentException("The key name cannot be empty!");
            if (!Contains(sectionName)) Add(sectionName);
            if (!Contains(sectionName, keyName))
                GetSection(sectionName).Keys.Add(new Key {Name = keyName, Value = keyValue});
        }

        /// <summary>
        /// Adds a range of blank sections to the file.
        /// </summary>
        /// <param name="sections">The sections to add.</param>
        public void AddRange(IEnumerable<String> sections)
        {
            foreach (var section in sections)
                Add(section);
        }

        /// <summary>
        /// Adds a range of sections to the file.
        /// </summary>
        /// <param name="sections">The sections to add.</param>
        public void AddRange(IEnumerable<Section> sections)
        {
            foreach (var section in sections)
                Add(section);
        }

        /// <summary>
        /// Adds a range of keys to a section.
        /// </summary>
        /// <param name="sectionName">The section to add the keys to.</param>
        /// <param name="keys">A list of keys to add.</param>
        public void AddRange(string sectionName, IEnumerable<Key> keys)
        {
            // Loop!
            foreach (var item in keys)
                Add(sectionName, item.Name, item.Value);
        }

        /// <summary>
        /// Adds a range of keys to a section.
        /// </summary>
        /// <typeparam name="T">The type of keys being added.</typeparam>
        /// <param name="sectionName">The name of the section.</param>
        /// <param name="keys">A Dictionary of keys and values to add.</param>
        public void AddRange<T>(string sectionName, IDictionary<String, T> keys)
        {
            foreach (var item in keys.Keys)
                Add(sectionName, item, keys[item]);
        }

        /// <summary>
        /// Inserts a section at the appropriate index.
        /// </summary>
        /// <param name="section">The section to insert.</param>
        /// <param name="index">The index to add the item at.</param>
        public void Insert(int index, Section section)
        {
            if (!Contains(section.Name))
                _sections.Insert(index, section);
        }

        /// <summary>
        /// Inserts a section at the appropriate index.
        /// </summary>
        /// <typeparam name="T">The type of key value being added.</typeparam>
        /// <param name="index">The index to add the item at.</param>
        /// <param name="sectionName">The name of the section.</param>
        /// <param name="keyName">The name of the key.</param>
        /// <param name="keyValue">The value of the key.</param>
        public void Insert<T>(int index, string sectionName, string keyName, T keyValue)
        {
            var section = new Section
                              {Name = sectionName, Keys = new List<Key> {new Key {Name = keyName, Value = keyValue}}};
            Insert(index, section);
        }

        #endregion

        #region Remove

        /// <summary>
        /// Remove a section from the INI file.
        /// </summary>
        /// <param name="sectionName">The name of the section to remove.</param>
        public bool Remove(string sectionName)
        {
            if (Contains(sectionName))
            { _sections.Remove(GetSection(sectionName)); return true; }
            return false;
        }

        /// <summary>
        /// Removes a key from the INI file.
        /// </summary>
        /// <param name="sectionName">The section to check under.</param>
        /// <param name="keyName">The key to remove.</param>
        public bool Remove(string sectionName, string keyName)
        {
            if (Contains(sectionName, keyName))
            { GetSection(sectionName).Keys.Remove(GetKey(sectionName, keyName)); return true; }
            return false;
        }

        /// <summary>
        /// Removes a section from the INI file.
        /// </summary>
        /// <param name="section">The section to remove.</param>
        /// <returns></returns>
        public bool Remove(Section section)
        {
            return Remove(section.Name);
        }

        /// <summary>
        /// Removes a section at the specified index.
        /// </summary>
        /// <param name="index">The index to check for.</param>
        /// <exception cref="IndexOutOfRangeException" />
        public void RemoveAt(int index)
        {
            if (index > -1 && index < _sections.Count())
                _sections.RemoveAt(index);
            throw new IndexOutOfRangeException("The value of index is not valid.");
        }

        /// <summary>
        /// Clears the sections out of the INI file.
        /// </summary>
        public void Clear()
        {
            _sections.Clear();
        }

        #endregion

        #region Edit

        /// <summary>
        /// Modifies a section name.
        /// </summary>
        /// <param name="sectionName">The old name of the section.</param>
        /// <param name="newName">The new name of the section.</param>
        public void SetSectionName(string sectionName, string newName)
        {
            if (Contains(sectionName))
                GetSection(sectionName).Name = newName;
        }

        /// <summary>
        /// Modifies the name of a key in the INI file.
        /// </summary>
        /// <param name="sectionName">The section to check under.</param>
        /// <param name="oldKeyName">The old key name.</param>
        /// <param name="newKeyName">The new key name.</param>
        public void SetKeyName(string sectionName, string oldKeyName, string newKeyName)
        {
            if (Contains(sectionName, oldKeyName))
                GetKey(sectionName, oldKeyName).Name = newKeyName;
        }

        /// <summary>
        /// Edits the value of a key.
        /// </summary>
        /// <param name="sectionName">The section to look under.</param>
        /// <param name="keyName">The key to edit.</param>
        /// <param name="newValue">The value to modify.</param>
        public void SetKeyValue<T>(string sectionName, string keyName, T newValue)
        {
            if (!Contains(sectionName, keyName))
                Add(sectionName, keyName, newValue);
            else
                GetKey(sectionName, keyName).Value = newValue;
        }

        #endregion

        #region Exist

        /// <summary>
        /// Checks to see if a section exists.
        /// </summary>
        /// <param name="sectionName">The name of the section.</param>
        /// <returns>Boolean</returns>
        public bool Contains(string sectionName)
        {
            return _sections.Exists(s => s.Name.Equals(sectionName));
        }

        /// <summary>
        /// Checks to see if a key exists in a specific section.
        /// </summary>
        /// <param name="sectionName">The section to check.</param>
        /// <param name="keyName">The key to look for.</param>
        /// <returns>Boolean</returns>
        public bool Contains(string sectionName, string keyName)
        {
            return Contains(sectionName) && GetSection(sectionName).Keys.Exists(k => k.Name.Equals(keyName));
        }

        /// <summary>
        /// Checks to see if a section exists.
        /// </summary>
        /// <param name="section">The section to check for.</param>
        /// <returns></returns>
        public bool Contains(Section section)
        {
            return _sections.Contains(section);
        }

        #endregion

        #region Retrieve

        /// <summary>
        /// Returns an integer that contains the index of the given section name.
        /// </summary>
        /// <param name="sectionName">The section name to check for.</param>
        /// <returns>An Integer.</returns>
        public int IndexOf(string sectionName)
        {
            return Contains(sectionName) ? _sections.IndexOf(GetSection(sectionName)) : -1;
        }

        /// <summary>
        /// Returns an integer that contains the index of the given section.
        /// </summary>
        /// <param name="section">The section to look for.</param>
        /// <returns></returns>
        public int IndexOf(Section section)
        {
            return Contains(section) ? _sections.IndexOf(section) : -1;
        }
        
        /// <summary>
        /// Copies the sections to an array.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        public void CopyTo(Section[] array)
        {
            CopyTo(array, 0);
        }

        /// <summary>
        /// Copies the sections to an array.
        /// </summary>
        /// <param name="array">The array to copy to.</param>
        /// <param name="startIndex">The index to start at.</param>
        public void CopyTo(Section[] array, int startIndex)
        {

            //make sure the index is valid
            if (startIndex > -1 && startIndex <= _sections.Count() - 1)
            {

                // find out how many items we wanna copy.
                var countToCopy = _sections.Count() - startIndex;

                // Check the array to make sure we have enough space;
                // if so, pass it on.
                if (array.Count() >= countToCopy)
                    _sections.CopyTo(array, startIndex);

            }
        }

        /// <summary>
        /// Retrieves a section.
        /// </summary>
        /// <param name="sectionName">The name of the section to retrieve.</param>
        /// <exception cref="ArgumentException">When raised, the given section name does not exist in the collection.</exception>
        /// <returns>A Section class which contains a list of keys and values.</returns>
        public Section GetSection(string sectionName)
        {
            if(Contains(sectionName))
                return this[sectionName];
            throw new ArgumentException("The section does not exist!");
        }

        /// <summary>
        /// Returns an individual Key.
        /// </summary>
        /// <param name="sectionName">The section the key is in.</param>
        /// <param name="keyName">The name of the key.</param>
        /// <exception cref="ArgumentException">When raised, the given section and key combination does not exist in the collection.</exception>
        /// <returns>Returns a single key with a matching value.</returns>
        public Key GetKey(string sectionName, string keyName)
        {
            
            // check to see if everything exists
            if(Contains(sectionName) && Contains(sectionName, keyName))
                return GetSection(sectionName)[keyName];

            // No key exists? Throw an error
            throw new ArgumentException("The given section and key do not yield a valid key entry in the INI file!");

        }

        /// <summary>
        /// Returns a list of keys based on the section name.
        /// </summary>
        /// <param name="sectionName">The section to check for.</param>
        /// <returns>A list of keys in the <paramref name="sectionName"/></returns>
        /// <exception cref="ArgumentException">When raised, the given section does not exist in the collection.</exception>
        public IEnumerable<Key> GetKeys(string sectionName)
        {

            // Return either the section or a blank list of keys.
            if (Contains(sectionName))
                return GetSection(sectionName).Keys;
            throw new ArgumentException("This section does not exist!");

        }

        /// <summary>
        /// Returns the value of the key.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="sectionName">The section the key is located in.</param>
        /// <param name="keyName">The name of the key to look for.</param>
        /// <param name="defaultValue">If the section or key does not exist, this parameter will be returned.</param>
        public T GetKeyValue<T>(string sectionName, string keyName, T defaultValue)
        {
            // Look for the object
            if (Contains(sectionName, keyName))
            {

                // Get the key.
                var key = GetKey(sectionName, keyName);

                // Give the key.
                return ChangeType<T>(key.Value, typeof(T));

            }

            // Return the default value of the key.
            return ChangeType<T>(defaultValue, typeof(T));

        }

        /// <summary>
        /// A behind-the-scenes function that performs most type conversions.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="keyValue">The value of the object.</param>
        /// <param name="objtype">The type of the object.</param>
        /// <exception cref="InvalidCastException"/>
        /// <exception cref="FormatException" />
        /// <exception cref="OverflowException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <returns></returns>
        private static T ChangeType<T>(object keyValue, System.Type objtype)
        {
            var types = new[]
                            {
                                typeof (object), typeof (DBNull), typeof (bool), typeof (char), typeof (sbyte),
                                typeof (byte), typeof (short), typeof (ushort), typeof (int), typeof (uint),
                                typeof (long), typeof (ulong), typeof (float), typeof (double), typeof (decimal),
                                typeof (DateTime), typeof (object), typeof (string)
                            };

            if((objtype == null || keyValue == null) || (keyValue as IConvertible == null) || !types.Contains(objtype))
                throw new ArgumentException();

            //if (keyValue.GetType().IsAssignableFrom(typeof(IConvertible)) && objtype.IsAssignableFrom(typeof(IConvertible)))
            return (T)Convert.ChangeType(keyValue, objtype);
        }

        #endregion

        #region Loading

        /// <summary>
        /// Load an INI file into this instance of the class.
        /// </summary>
        /// <param name="location">The location of the INI file.</param>
        private void LoadINI(string location)
        {
            var fileData = File.ReadAllLines(location, Encoding.UTF8)
                    .Where(s => !string.IsNullOrEmpty(s) && !s.StartsWith(";")).ToList();

            // Begin looping to remove ; from ends
            for (var i = 0; i < fileData.Count(); i++)
                if (fileData[i].Contains(';')) fileData[i] = fileData[i].Split(';')[0].Trim();

            // Loop each line.
            var currentSection = string.Empty;
            foreach (var line in fileData)
            {
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentSection = line.Trim(new[] { '[', ']' }).Trim();
                    Add(currentSection);
                }
                else
                {

                    // we need to find the first index of the = sign.
                    var firstEqual = line.IndexOf('=');

                    // okay, we know the index of the first equal sign. substring!
                    var keyName = line.Substring(0, firstEqual).Trim();
                    var keyData = line.Remove(0, firstEqual + 1);

                    // Redo the keyData
                    if (keyData.Contains("%n"))
                        keyData = keyData.Replace("%n", Environment.NewLine);

                    // Parse time
                    bool boolDumb;
                    decimal decDumb;

                    if (Boolean.TryParse(keyData, out boolDumb))
                    { Add(currentSection, keyName, boolDumb); continue; }
                    if (Decimal.TryParse(keyData, out decDumb))
                    { Add(currentSection, keyName, decDumb); continue; }

                    Add(currentSection, keyName, keyData);

                }
            }
        }

        /// <summary>
        /// This will load an XML file into the INI class.
        /// </summary>
        /// <param name="location">The XML file to load.</param>
        private void LoadXML(string location)
        {

            // Load in the XML document.
            var xmlDoc = XDocument.Load(location);

            // Loop for each of the sections that are inside this file.
            foreach (var section in xmlDoc.Descendants("Section"))
            {

                // The first element is the name of this section
                var sectionNameNode = section.Elements().First();

                // Define our section
                var iniSection = new Section {Name = sectionNameNode.Value, Keys = new List<Key>()};

                // Loop to get each key that will be added.
                foreach (var xElement in section.Descendants("Keys").SelectMany(key => key.Elements()))
                {

                    // Define our key.
                    var key = new Key {Comment = ""};

                    // Loop for each sub key.
                    foreach (var descendant in xElement.Descendants())
                    {
                        if (descendant.Name.ToString().ToLowerInvariant() == "name")
                            key.Name = descendant.Value;
                        else
                            key.Value = descendant.Value;
                    }

                    // Add
                    iniSection.Add(key);

                }

                // Add this section to the INI file
                Add(iniSection);

            }

        }

        /// <summary>
        /// Loads an INI file (or an XML file that was generated by this class).
        /// </summary>
        /// <param name="location">The file path of the INI/XML file.</param>
        /// <param name="loadType">The type of file to load.</param>
        public void Load(string location, Type loadType)
        {
            if (loadType == Type.INI)
                LoadINI(location);
            else
                LoadXML(location);
        }

        #endregion
        
        #region Saving

        /// <summary>
        /// Exports this class into an INI file.
        /// </summary>
        /// <param name="sw">The stream to write the file to.</param>
        private void SaveINI(TextWriter sw)
        {
            foreach (var section in this)
            {
                sw.WriteLine(string.Format("[{0}]", section.Name));
                foreach (var key in section)
                {
                    var keyValue = key.Value != null ? key.Value.ToString() : "null";
                    keyValue = keyValue.Replace("\r\n", "%n");
                    sw.WriteLine("{0}={1} {2}", key.Name, keyValue, String.IsNullOrEmpty(key.Comment) ? "" : String.Format("; {0}", key.Comment));
                }
                sw.WriteLine();
            }
        }

        /// <summary>
        /// Exports this class into an XML file.
        /// </summary>
        /// <param name="sw">The stream to write the file to.</param>
        private void SaveXML(TextWriter sw)
        {
            // create the new settings
            var settings = new XmlWriterSettings
            {
                Indent = true,
                CheckCharacters = true,
                CloseOutput = false,
                ConformanceLevel = ConformanceLevel.Document,
                Encoding = Encoding.UTF8,
                OmitXmlDeclaration = false
            };


            using (var writer = XmlWriter.Create(sw, settings))
            {

                // set a few settings.
                writer.WriteStartDocument();
                writer.WriteStartElement("Sections");
                foreach (var section in this)
                {
                    // <section>
                    writer.WriteStartElement("Section");
                    writer.WriteElementString("name", section.Name);

                    // <keys>
                    writer.WriteStartElement("Keys");
                    foreach (var key in section)
                    {
                        // <key>
                        writer.WriteStartElement("Key");
                        writer.WriteElementString("name", key.Name);
                        writer.WriteElementString("value", key.Value.ToString());
                        writer.WriteEndElement();
                        // </key>
                    }

                    // </keys>
                    writer.WriteEndElement();

                    // </section>
                    writer.WriteEndElement();

                }

                // </sections>
                writer.WriteEndElement();
                writer.WriteEndDocument();

            }
        }

        /// <summary>
        /// Saves an INI file to the disk (or optionally an XML representation)
        /// </summary>
        /// <param name="location">The file path to save to.</param>
        /// <param name="saveType">The type of file to save.</param>
        public void Save(string location, Type saveType)
        {
            var stream = new StreamWriter(location, false, Encoding.UTF8);
            if (saveType == Type.INI)
                SaveINI(stream);
            else
                SaveXML(stream);

            stream.Flush();
            stream.Close();
        }

        #endregion

        #region Merging

        /// <summary>
        /// This routine will merge the currently loaded file with another file.
        /// </summary>
        /// <param name="location">The location of the file.</param>
        /// <param name="fileType">The type of the file, whether it be INI or XML.</param>
        public void Merge(string location, Type fileType)
        {
            AddRange(new INI(location, fileType));
        }

        /// <summary>
        /// This routine will merge numerous INI or XML files into the currently loaded file.
        /// </summary>
        /// <param name="locations">The locations of INI or XML files to merge.</param>
        /// <param name="fileType">The type of the files to be merged.</param>
        public void Merge(IEnumerable<string> locations, Type fileType)
        {
            foreach (var location in locations)
                Merge(location, fileType);
        }

        /// <summary>
        /// This routine will merge numerous INI or XML files into the currently loaded file.
        /// </summary>
        /// <param name="files">A dictionary containing the file path and type to be loaded.</param>
        public void Merge(IDictionary<string, Type> files)
        {
            foreach (var item in files.Keys)
                Merge(item, files[item]);
        }

        #endregion

        #region Enumerator Specifics

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Section> GetEnumerator()
        {
            return new SectionEnumerator(_sections);
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new SectionEnumerator(_sections);
        }

        #endregion

        #region IEnumerator

        /// <summary>
        /// Custom enumerator for the IniLibrary.
        /// </summary>
        private class SectionEnumerator : IEnumerator<Section>
        {
            private readonly Section[] _sections;
            private int _index = -1;

            public SectionEnumerator(IEnumerable<Section> data)
            {
                _sections = data.ToArray();
            }

            public Section Current
            {
                get { return _sections[_index]; }
            }

            public void Dispose()
            {

            }

            object System.Collections.IEnumerator.Current
            {
                get { return _sections[_index]; }
            }

            public bool MoveNext()
            {
                _index++;
                return _index <= _sections.Count() - 1;
            }

            public void Reset()
            {
                _index = -1;
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Returns a string representation of this ini file.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("INI File: {0}Section Count: {1}{0}Key Count:{2}", Environment.NewLine,
                                 Count, _sections.Select(k => k.Keys).Count());
        }

        #endregion

    }

}