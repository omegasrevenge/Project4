#define READABLE
 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
 
/*
 * http://www.opensource.org/licenses/lgpl-2.1.php
 * JSONObject class
 * for use with Unity
 * Copyright Matt Schoen 2010
 */
 
public class JSONObject : Nullable {
	public enum Type { NULL, STRING, NUMBER, OBJECT, ARRAY, BOOL }
	public bool isContainer { get { return (type == Type.ARRAY || type == Type.OBJECT); }}
	public JSONObject parent;
	public Type type = Type.NULL;
	public int Count { get { return list.Count; } }
	public ArrayList list = new ArrayList();
	public ArrayList keys = new ArrayList();
	public string str;
	public double n;
	public bool b;
 
	public static JSONObject nullJO { get { return new JSONObject(JSONObject.Type.NULL); } }
	public static JSONObject obj { get { return new JSONObject(JSONObject.Type.OBJECT); } }
	public static JSONObject arr { get { return new JSONObject(JSONObject.Type.ARRAY); } }
 
	public JSONObject(JSONObject.Type t) {
		type = t;
		switch(t) {
		case Type.ARRAY:
			list = new ArrayList();
			break;
		case Type.OBJECT:
			list = new ArrayList();
			keys = new ArrayList();
			break;
		}
	}
	public JSONObject(bool b) {
		type = Type.BOOL;
		this.b = b;
	}
	public JSONObject(float f) {
		type = Type.NUMBER;
		this.n = f;
	}
	public JSONObject(Dictionary<string, string> dic) {
		type = Type.OBJECT;
		foreach(KeyValuePair<string, string> kvp in dic){
			keys.Add(kvp.Key);
			list.Add(new JSONObject { type = Type.STRING, str = kvp.Value });
		}
	}
	public void Absorb(JSONObject obj){
		list.AddRange(obj.list);
		keys.AddRange(obj.keys);
		str = obj.str;
		n = obj.n;
		b = obj.b;
		type = obj.type;
	}
	public JSONObject() {}

	public static explicit operator string(JSONObject o)
	{
		if(o==null) return null;
		switch(o.type)
		{
		case Type.STRING: return o.str;
		case Type.NUMBER: return ""+o.n;
		case Type.BOOL: return o.b?"true":"false";
		case Type.ARRAY: return "Array";
		case Type.OBJECT: return "Object";
		case Type.NULL: return "null";
		};
		return "Unknown";
	}
	public static explicit operator bool(JSONObject o)
	{
		if(o==null) return false;
		switch(o.type)
		{
		case Type.STRING: return true;
		case Type.NUMBER: return o.n!=0;
		case Type.BOOL: return o.b;
		case Type.ARRAY: return true;
		case Type.OBJECT: return true;
		};
		return false;
	}
	public static explicit operator double(JSONObject o)
	{
		if(o==null) return 0;
		switch(o.type)
		{
		case Type.STRING: return double.Parse(o.str);
		case Type.NUMBER: return o.n;
		case Type.BOOL: return o.b?1:0;
		};
		return 0;
	}
	public static explicit operator float(JSONObject o)
	{
		if(o==null) return 0;
		switch(o.type)
		{
		case Type.STRING: return float.Parse(o.str);
		case Type.NUMBER: return (float)o.n;
		case Type.BOOL: return o.b?1:0;
		};
		return 0;
	}
	public static explicit operator int(JSONObject o)
	{
		if(o==null) return 0;
		switch(o.type)
		{
		case Type.STRING: return int.Parse(o.str);
		case Type.NUMBER: return (int)o.n;
		case Type.BOOL: return o.b?1:0;
		};
		return 0;
	}
	public static explicit operator long(JSONObject o)
	{
		if(o==null) return 0;
		switch(o.type)
		{
		case Type.STRING: return long.Parse(o.str);
		case Type.NUMBER: return (long)o.n;
		case Type.BOOL: return o.b?1:0;
		};
		return 0;
	}
	public static explicit operator DateTime(JSONObject o)
	{
		System.DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0);
		if(o==null) return dtDateTime;
		long unixTimeStamp=((long)o)/(1000000000);
		return dtDateTime.AddSeconds(unixTimeStamp);
	}

	public JSONObject(string s) {type = Type.STRING;str = s;}
	public void Add(bool val) { Add(new JSONObject(val)); }
	public void Add(float val) { Add(new JSONObject(val)); }
	public void Add(int val) { Add(new JSONObject(val)); }
	public void Add(JSONObject obj) {
		if(obj) {		//Don't do anything if the object is null
			if(type != JSONObject.Type.ARRAY) {
				type = JSONObject.Type.ARRAY;		//Congratulations, son, you're an ARRAY now
				Debug.LogWarning("tried to add an object to a non-array JSONObject.  We'll do it for you, but you might be doing something wrong.");
			}
			list.Add(obj);
		}
	}
	public void AddField(string name, bool val) { AddField(name, new JSONObject(val)); }
	public void AddField(string name, float val) { AddField(name, new JSONObject(val)); }
	public void AddField(string name, int val) { AddField(name, new JSONObject(val)); }
	public void AddField(string name, string val) {
		AddField(name, new JSONObject { type = JSONObject.Type.STRING, str = val });
	}
	public void AddField(string name, JSONObject obj) {
		if(obj){		//Don't do anything if the object is null
			if(type != JSONObject.Type.OBJECT){
				type = JSONObject.Type.OBJECT;		//Congratulations, son, you're an OBJECT now
				Debug.LogWarning("tried to add a field to a non-object JSONObject.  We'll do it for you, but you might be doing something wrong.");
			}
			keys.Add(name);
			list.Add(obj);
		}
	}
	public void SetField(string name, JSONObject obj) {
		if(HasField(name)) {
			list.Remove(this[name]);
			keys.Remove(name);
		}
		AddField(name, obj);
	}
	public JSONObject GetField(string name) {
		if(type == JSONObject.Type.OBJECT)
			for(int i = 0; i < keys.Count; i++)
				if((string)keys[i] == name)
					return (JSONObject)list[i];
		return null;
	}
	public bool HasField(string name) {
		if(type == JSONObject.Type.OBJECT)
			for(int i = 0; i < keys.Count; i++)
				if((string)keys[i] == name)
					return true;
		return false;
	}
	public void Clear() {
		type = JSONObject.Type.NULL;
		list.Clear();
		keys.Clear();
		str = "";
		n = 0;
		b = false;
	}
	public JSONObject Copy() {
		//return JSONParser.parse(print()); //this is slower than your grandma
		
		JSONObject xCopy=new JSONObject(type);
		xCopy.Merge(this);	//simple, eh. but a 100 times faster because it won't torture the fuckin' garbage collector!
		
		return xCopy;
	}
	/*
	 * The Merge function is experimental. Use at your own risk.
	 */
	public void Merge(JSONObject obj) {
		MergeRecur(this, obj);
	}
	/// <summary>
	/// Merge object right into left recursively
	/// </summary>
	/// <param name="left">The left (base) object</param>
	/// <param name="right">The right (new) object</param>
	static void MergeRecur(JSONObject left, JSONObject right) {
		if(left.type == JSONObject.Type.NULL)
			left.Absorb(right);
		else if(left.type == Type.OBJECT && right.type == Type.OBJECT) {
			for(int i = 0; i < right.list.Count; i++) {
				string key = (string)right.keys[i];
				if(right[i].isContainer){
					if(left.HasField(key))
						MergeRecur(left[key], right[i]);
					else
						left.AddField(key, right[i]);
				} else {
					if(left.HasField(key))
						left.SetField(key, right[i]);
					else
						left.AddField(key, right[i]);
				}
			}
		} else if(left.type == Type.ARRAY && right.type == Type.ARRAY) {
			if(right.Count > left.Count){
				Debug.LogError("Cannot merge arrays when right object has more elements");
				return;
			}
			for(int i = 0; i < right.list.Count; i++) {
				if(left[i].type == right[i].type) {			//Only overwrite with the same type
					if(left[i].isContainer)
						MergeRecur(left[i], right[i]);
					else{
						left[i] = right[i];
					}
				}
			}
		}
	}
	public string print() {
		return print(0);
	}
	public string print(int depth) {	//Convert the JSONObject into a string
		if(depth++ > 1000) {
			Debug.Log("reached max depth!");
			return "";
		}
		string str = "";
		switch(type) {
		case Type.STRING:
			str = "\"" + this.str + "\"";
			break;
		case Type.NUMBER:
			if(n == Mathf.Infinity)
				str = "+Inf";
			else if(n == Mathf.NegativeInfinity)
				str = "-Inf";
			else
				str += n;
			break;
		case JSONObject.Type.OBJECT:
			if(list.Count > 0) {
				str = "{";
#if(READABLE)	//for a bit more readability, comment the define above to save space
				str += "\n";
				depth++;
#endif
				for(int i = 0; i < list.Count; i++) {
					//Debug.Log(">>"+type+" "+list.Count+" "+keys.Count);
					if(i>=keys.Count) {break;};
					string key = (string)keys[i];
					JSONObject obj = (JSONObject)list[i];
					if(obj) {
#if(READABLE)
						for(int j = 0; j < depth; j++)
							str += "\t"; //for a bit more readability
#endif
						str += "\"" + key + "\":";
						str += obj.print(depth) + ",";
#if(READABLE)
						str += "\n";
#endif
					}
				}
#if(READABLE)
				str = str.Substring(0, str.Length - 1);
#endif
				str = str.Substring(0, str.Length - 1);
				str += "}";
			} else str = "null";
			break;
		case JSONObject.Type.ARRAY:
			if(list.Count > 0) {
				str = "[";
#if(READABLE)
				str += "\n"; //for a bit more readability
				depth++;
#endif
				foreach(JSONObject obj in list) {
					if(obj) {
#if(READABLE)
						for(int j = 0; j < depth; j++)
							str += "\t"; //for a bit more readability
#endif
						str += obj.print(depth) + ",";
#if(READABLE)
						str += "\n"; //for a bit more readability
#endif
					}
				}
#if(READABLE)
				str = str.Substring(0, str.Length - 1);
#endif
				str = str.Substring(0, str.Length - 1);
				str += "]";
			} else str = "null";
			break;
		case Type.BOOL:
			if(b)
				str = "true";
			else
				str = "false";
			break;
		case Type.NULL:
			str = "null";
			break;
		}
		return str;
	}
	public JSONObject this[int index] {
		get {
			if(list.Count > index)	return (JSONObject)list[index];
			else 					return null;
		}
		set {
			if(list.Count > index)
				list[index] = value;
		}
	}
	public JSONObject this[string index] {
		get {
			return GetField(index);
		}
		set {
			SetField(index, value);
		}
	}
	public override string ToString() {
		return print();
	}
	public Dictionary<string, string> ToDictionary() {
		if(type == Type.OBJECT) {
			Dictionary<string, string> result = new Dictionary<string, string>();
			for(int i = 0; i < list.Count; i++) {
				JSONObject val = (JSONObject)list[i];
				switch(val.type){
				case Type.STRING:	result.Add((string)keys[i], val.str);		break;
				case Type.NUMBER:	result.Add((string)keys[i], val.n + "");	break;
				case Type.BOOL:		result.Add((string)keys[i], val.b + "");	break;
				default: Debug.LogWarning("Omitting object: " + (string)keys[i] + " in dictionary conversion"); break;
				}
			}
			return result;
		} else Debug.LogWarning("Tried to turn non-Object JSONObject into a dictionary");
		return null;
	}
}

public class JSONParser
{
	enum JsonToken
	{
        None = 0,
        CurlyOpen = 1,
        CurlyClose = 2,
        SquaredOpen = 3,
        SquaredClose = 4,
        Colon = 5,
        Comma = 6,
        String = 7,
        Number = 8,
        True = 9,
        False = 10,
        Null = 11,
		Letter = 12,
	};

	public static JSONObject parse(string json)
	{
        bool success = true;
		if(json==null||json.Length<=0) {return null;};
		json=json.Replace("\r","");
        int index = 0;
		JSONObject value = parseValue(json,ref index,ref success);
		return success?value:null;
	}

	static void GetLineCol(string json,int index,ref int line,ref int col)
	{
		line=0;
		col=0;
		for(int i=0;i<json.Length&&i<index;i++)
		{
			bool bNewLine=json[i]=='\n';
			if(bNewLine) {line++;col=0;} else {col++;}
		}
	}

	static void Error(string json,int index,string reason)
	{
		int line=0;
		int col=0;
		GetLineCol(json,index,ref line,ref col);
		Debug.LogError("JSON parse, ln:"+line+" col:"+col+" - "+reason);
	}

	static JSONObject parseValue(string json,ref int index,ref bool success)
	{
		JSONObject jobj;
        //Determine what kind of data we should parse by
        //checking out the upcoming token
		JsonToken tok=lookAhead(json,ref index);
        switch(tok)
        {
            case JsonToken.String:
                    return parseString(json,ref index,ref success);
            case JsonToken.Number:
                    return parseNumber(json,ref index);
            case JsonToken.CurlyOpen:
                    return parseObject(json,ref index,ref success);
            case JsonToken.SquaredOpen:
                    return parseArray(json,ref index,ref success);
            case JsonToken.True:
                    nextToken(json,ref index);
					jobj=new JSONObject();
					jobj.type = JSONObject.Type.BOOL;
					jobj.b=true;
                    return jobj;
            case JsonToken.False:
                    nextToken(json,ref index);
					jobj=new JSONObject();
					jobj.type = JSONObject.Type.BOOL;
					jobj.b=false;
                    return jobj;
            case JsonToken.Null:
                    nextToken(json,ref index);
					jobj=new JSONObject();
					jobj.type = JSONObject.Type.NULL;
                    return jobj;
            case JsonToken.None:
                    break;
        }

        //If there were no tokens, flag the failure and return an empty QVariant
        success = false;
		Error(json,index,"invtoken:"+tok.ToString());
        return null;
	}

	static JSONObject parseObject(string json,ref int index,ref bool success)
	{
		JSONObject jobj=new JSONObject();
		jobj.type = JSONObject.Type.OBJECT;

        JsonToken token;

        //Get rid of the whitespace and increment index
        nextToken(json,ref index);

        //Loop through all of the key/value pairs of the object
        bool done = false;
        while(!done)
        {
            //Get the upcoming token
            token = lookAhead(json,ref index);

            if(token==JsonToken.None)
            {
				success = false;
				Error(json,index,"Object: invtoken");
                return jobj;
            }
            else if(token == JsonToken.Comma)
            {
                nextToken(json,ref index);
            }
            else if(token == JsonToken.CurlyClose)
            {
                nextToken(json,ref index);
                return jobj;
            }
            else
            {
                //Parse the key/value pair's name
                string name = parseString(json,ref index,ref success).str;
                if(!success) {return jobj;}
                //Get the next token
                token = nextToken(json,ref index);

                //If the next token is not a colon, flag the failure
                //return an empty QVariant
                if(token != JsonToken.Colon)
                {
                    success = false;
					Error(json,index,"Object: colon expected");
                    return jobj;
                }

                //Parse the key/value pair's value
                JSONObject value = parseValue(json,ref index,ref success);
				if(!success) {return jobj;}


                //Assign the value to the key in the map
				jobj.keys.Add(name);
				jobj.list.Add(value);
            }
        }

        //Return the map successfully
        return jobj;
	}

	static JSONObject parseArray(string json,ref int index,ref bool success)
	{
		JSONObject jobj=new JSONObject();
		jobj.type = JSONObject.Type.ARRAY;

        nextToken(json,ref index);

        bool done = false;
        while(!done)
        {
            JsonToken token = lookAhead(json,ref index);

            if(token == JsonToken.None)
            {
                success = false;
                return jobj;
            }
            else if(token == JsonToken.Comma)
            {
                nextToken(json,ref index);
            }
            else if(token == JsonToken.SquaredClose)
            {
                nextToken(json,ref index);
                break;
            }
            else
            {
                JSONObject value = parseValue(json,ref index,ref success);
                if(!success) {return jobj;}
                jobj.list.Add(value);
            }
        }

        return jobj;
	}

	static JSONObject parseString(string json,ref int index,ref bool success)
	{
	    string s="";
	    char c;

	    eatWhitespace(json,ref index);

	    c = json[index++];
		bool bQuoted=(c=='"');
		if(!bQuoted) {index--;};

	    bool complete = false;
	    while(!complete)
	    {
            if(index == json.Length) {break;};

            c = json[index++];

			if(!bQuoted)
			{
                if (!char.IsLetterOrDigit(c) && c != '_' && c != '-')
				{
					complete=true;
					index--;
					break;
				};
			};

            if(c == '\"')
            {
                complete = true;
                break;
            }
            else if(c == '\\')
            {
                if(index == json.Length)
                {
                    break;
                }

                c = json[index++];

                if(c == '\"')
                {
                    s+=('\"');
                }
                else if(c == '\\')
                {
                    s+=('\\');
                }
                else if(c == '/')
                {
                    s+=('/');
                }
                else if(c == 'b')
                {
                    s+=('\b');
                }
                else if(c == 'f')
                {
                    s+=('\f');
                }
                else if(c == 'n')
                {
                    s+=('\n');
                }
                else if(c == 'r')
                {
                    s+=('\r');
                }
                else if(c == 't')
                {
                    s+=('\t');
                }
            }
            else
            {
                s+=(c);
            }
	    }

	    if(!complete)
	    {
            success = false;
			Error(json,index,"String: incomplete");
            return null;
	    }

		JSONObject jobj=new JSONObject();
		jobj.str = s;
		jobj.type = JSONObject.Type.STRING;
		return jobj;
	}

	static JSONObject parseNumber(string json,ref int index)
	{
        eatWhitespace(json,ref index);

        int lastIndex = lastIndexOfNumber(json,ref index);
        int charLength = (lastIndex - index) + 1;
        string numberStr;

        numberStr = json.Substring(index, charLength);

        index = lastIndex + 1;

		JSONObject jobj=new JSONObject();
		jobj.n = System.Convert.ToDouble(numberStr);
		jobj.type = JSONObject.Type.NUMBER;
		return jobj;
	}

	static int lastIndexOfNumber(string json,ref int index)
	{
        int lastIndex;

        for(lastIndex = index; lastIndex < json.Length; lastIndex++)
        {
            if(("0123456789+-.eE").IndexOf(json[lastIndex]) == -1)
            {
                break;
            }
        }

        return lastIndex -1;
	}

	static void eatWhitespace(string json,ref int index)
	{
		bool bInLineComment=false;
		bool bInBlockComment=false;

        for(; index < json.Length; index++)
        {
			char c=json[index];
			if(bInLineComment&&c=='\n') {bInLineComment=false;continue;};
			if(bInBlockComment&&c=='*'&&index<json.Length-1&&json[index+1]=='/') {bInBlockComment=false;index++;continue;};
			if(bInLineComment||bInBlockComment) {continue;};

            if((" \t\n\r").IndexOf(c) == -1)
            {
				//line comment, skip to end of line
				if(c=='/'&&index<json.Length-1)
				{
					if(json[index+1]=='/') {bInLineComment=true;index++;};
					if(json[index+1]=='*') {
						bInBlockComment=true;index++;
					};
				};

				if(!bInLineComment&&!bInBlockComment) {break;};
            };
        };
	}

	static JsonToken lookAhead(string json,ref int index)
	{
        int saveIndex = index;
        return nextToken(json,ref saveIndex);
	}

	static JsonToken nextToken(string json,ref int index)
	{
        eatWhitespace(json,ref index);

        if(index == json.Length)
        {
			return JsonToken.None;
        }

        char c = json[index];
        index++;
        switch(c)
        {
            case '{': return JsonToken.CurlyOpen;
            case '}': return JsonToken.CurlyClose;
            case '[': return JsonToken.SquaredOpen;
            case ']': return JsonToken.SquaredClose;
            case ',': return JsonToken.Comma;
            case '"': return JsonToken.String;
            case '0': case '1': case '2': case '3': case '4':
            case '5': case '6': case '7': case '8': case '9':
            case '-': return JsonToken.Number;
            case ':': return JsonToken.Colon;
        }
        index--;

        int remainingLength = json.Length - index;

        //True
        if(remainingLength >= 4)
        {
            if (json[index] == 't' && json[index + 1] == 'r' &&
                    json[index + 2] == 'u' && json[index + 3] == 'e')
            {
                index += 4;
                return JsonToken.True;
            }
        }

        //False
        if (remainingLength >= 5)
        {
            if (json[index] == 'f' && json[index + 1] == 'a' &&
                    json[index + 2] == 'l' && json[index + 3] == 's' &&
                    json[index + 4] == 'e')
            {
                index += 5;
                return JsonToken.False;
            }
        }

        //Null
        if (remainingLength >= 4)
        {
            if (json[index] == 'n' && json[index + 1] == 'u' &&
                    json[index + 2] == 'l' && json[index + 3] == 'l')
            {
                index += 4;
                return JsonToken.Null;
            }
        }

		if(char.IsLetter(c)) {index++;return JsonToken.Letter;};


        return JsonToken.None;
	}
}

