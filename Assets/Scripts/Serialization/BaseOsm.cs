using System;
using System.Globalization;
using System.Xml;


/// Этот класс наследуется другими классами в папке сериализации
/// и используется для извлечения данных из XML-файлов OSM.

class BaseOsm
{
    /// <typeparam name="T">data type</typeparam>
    /// <param name="attrName">name of the attribute</param>
    /// <param name="attributes">the collection of attributes within the data</param>
    /// <returns>The value of the attribute converted to the required type</returns>
    protected T GetAttribute<T>(string attrName, XmlAttributeCollection attributes)
    {
        string strValue = attributes[attrName].Value;
        return (T)Convert.ChangeType(strValue, typeof(T));
    }

    /// <param name="attrName">name of the attribute</param>
    /// <param name="attributes">the collection of attributes within the data</param>
    /// <returns>The value of the attribute converted to float</returns>
    protected float GetFloat(string attrName, XmlAttributeCollection attributes)
    {
        string strValue = attributes[attrName].Value;
        return float.Parse(strValue, new CultureInfo("en-US").NumberFormat);
    }
}



