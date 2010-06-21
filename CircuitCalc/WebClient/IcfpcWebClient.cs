﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace CircuitCalc.WebClient
{
	public class IcfpcWebClient
	{
		//TODO: login почему-то не фурычит
		public IcfpcWebClient(string sessionId)
		{
			this.sessionId = string.IsNullOrEmpty(sessionId) ? Login() : sessionId;
		}

		public IEnumerable<string> GetCarIdsList()
		{
			return GetCarIdsList(10000);
		}

		public IEnumerable<string> GetCarIdsList(int maxPages)
		{
			var carIds = new HashSet<string>();
			for(int page = 1; page < maxPages; page++)
			{
				var response = GetResponse(string.Format(getPagedCarsList, page));
				if(response.Contains("No cars found."))
				{
					Console.WriteLine("pages: {0}", page - 1);
					break;
				}
				foreach(var carId in HtmlParser.ParseCarsList(response))
				{
					carIds.Add(carId);
				}
			}
			return carIds.ToArray();
		}

		public Dictionary<string, string> GetCarsList()
		{
			var carIds = GetCarIdsList();
			var cars = new Dictionary<string, string>();
			foreach(var carId in carIds)
			{
				var car = GetCar(carId);
				cars.Add(carId, car);
			}
			return cars;
		}

		public string GetCar(string carId)
		{
			var response = GetResponse(string.Format(getCar, carId));
			//File.WriteAllText(string.Format("car-{0}.html", carId), response);
			var car = HtmlParser.ParseCar(response);
			return car;
		}

		public SubmitFuelResponse SubmitFuel(string carId, string factory)
		{
			var response = Post(string.Format(submitFuel, carId), Escape(factory));
			File.WriteAllText(string.Format("submit-fuel-for-car-{0}.html", carId), response);
			var result = HtmlParser.ParseSubmitFuelResponse(response);
			return result;
		}

		public string TestFactory(string factory)
		{
			return Post(submitFuelFake, Escape(factory), "G1");
		}

		private static string Escape(string factory)
		{
			return factory.Replace("\r\n", "%0D%0A").Replace("#", "%23").Replace(",", "%2C").Replace(":", "%3A");
		}

		private string Post(string requestUri, string data)
		{
			return Post(requestUri, data, "contents");
		}

		private string Post(string requestUri, string data, string paramName)
		{
			var req = CreateWebRequest(requestUri);
			req.Method = "POST";
			req.ContentType = "application/x-www-form-urlencoded";
			using(var s = req.GetRequestStream())
			{
				var buffer = Encoding.ASCII.GetBytes(paramName + "=" + data);
				s.Write(buffer, 0, buffer.Length);
			}
			using(var resp = req.GetResponse())
				return GetContent(resp.GetResponseStream());
		}

		private static string Login()
		{
			var req = WebRequest.Create(login);
			req.Method = "POST";
			req.ContentType = "application/x-www-form-urlencoded";
			using(var s = req.GetRequestStream())
			{
				var buffer = Encoding.ASCII.GetBytes(string.Format("j_username={0}&j_password={1}", user, pwd));
				s.Write(buffer, 0, buffer.Length);
			}
			using(var resp = req.GetResponse())
			{
				var cookie = resp.Headers["Set-Cookie"];
				Debug.Assert(cookie.StartsWith("JSESSIONID="));
				return cookie.Split(new[] {';'})[0].Split(new[] {'='})[1];
			}
		}

		private string GetResponse(string requestUri)
		{
			var req = CreateWebRequest(requestUri);
			using(var resp = req.GetResponse())
				return GetContent(resp.GetResponseStream());
		}

		private WebRequest CreateWebRequest(string requestUri)
		{
			var req = WebRequest.Create(requestUri);
			req.Headers.Add("Cookie", string.Format("JSESSIONID={0}", sessionId));
			return req;
		}

		private static string GetContent(Stream stream)
		{
			using(var sr = new StreamReader(stream, Encoding.UTF8))
				return sr.ReadToEnd();
		}

		private readonly string sessionId;
		private const string user = @"hack-the-loop";
		private const string pwd = @"722750249482275797203643486818027156264822953884882105712868";
		private const string root = @"http://icfpcontest.org/icfp10";
		private const string login = @"http://icfpcontest.org/icfp10/login";
		private const string getCarsList = @"http://icfpcontest.org/icfp10/score/instanceTeamCount";
		private const string getPagedCarsList = @"http://icfpcontest.org/icfp10/score/instanceTeamCount?page={0}&size=10";
		private const string submitCar = @"http://icfpcontest.org/icfp10/instance/form";
		private const string getCar = @"http://icfpcontest.org/icfp10/instance/{0}/solve/form";
		private const string submitFuel = @"http://icfpcontest.org/icfp10/instance/{0}/solve";
		private const string submitFuelFake = @"http://nfa.imn.htwk-leipzig.de/icfpcont/?G0=0";
	}
}