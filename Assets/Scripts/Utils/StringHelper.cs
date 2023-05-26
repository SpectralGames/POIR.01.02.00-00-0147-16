using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StringHelper{

	public static string GetRandomString(int length = 8)
	{
		var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		var stringChars = new char[length];
		for (int i = 0; i < stringChars.Length; i++)
		{
			stringChars[i] = chars[UnityEngine.Random.Range(0, chars.Length)];
		}
		return new String(stringChars);
	}

    public static string GetRandomID(int length = 8, int step = 5)
    {
        string result = GetRandomString(length);
        for (int i = 0; i < step-1; i++)
        {
            result += "-" + GetRandomString(length);
        }
        return result;
    }

}
