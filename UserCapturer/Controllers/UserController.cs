using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using UserCapturer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UserCapturer.Controllers
{
    public class UserController : Controller
    {

        private readonly string xmlFilePath = "users.xml";
        private XDocument _xDocument;

        

        // GET: UserController
        public ActionResult Index()
        {
            List<User> listUsers = new List<User>();

            if (!System.IO.File.Exists(xmlFilePath))
            {                
                TempData["ErrorMessage"] = "There are no records to show, please capture your first record below.";
                return RedirectToAction("Create");
            }

            XDocument xDocument = XDocument.Load(xmlFilePath);

            var users = from user in xDocument.Root.Elements("User")
                        select new
                        {
                            Id = user.Element("Id").Value,
                            Name = user.Element("Name").Value,
                            Surname = user.Element("Surname").Value,
                            Username = user.Element("Username").Value,
                            Cellphone = user.Element("Cellphone").Value
                        };

            if (users.Any())
            {                
                foreach (var user in users)
                {
                    
                    listUsers.Add(new User { 
                        Id = user.Id,
                        Name = user.Name,
                        Surname = user.Surname,
                        Username = user.Username,
                        Cellphone = user.Cellphone
                    });
                }
            }
            else
            {
                
                TempData["ErrorMessage"] = "There are no records to show, please capture your first record below.";
                return RedirectToAction("Create");
            }

            TempData.Clear();
            return View(listUsers);
        }

        

        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateUserModel user, IFormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Surname) || string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Cellphone))
                    {                        
                        TempData["ErrorMessage"] = "Make sure all fields are filled.";
                        return View(user);
                    }
                    if (!CommonFunctions.IsValidPhoneNumber(user.Cellphone))
                    {
                        TempData["ErrorMessage"] = "Invalid Cellphone number.";
                        return View(user);
                    }

                    if (System.IO.File.Exists(xmlFilePath))
                    {
                        var xmlDoc = XDocument.Load(xmlFilePath);
                        int nextId = CommonFunctions.GetNextId(xmlDoc);

                        XElement userElement = new XElement("User",
                            new XElement("Id", nextId),
                            new XElement("Name", user.Name),
                            new XElement("Surname", user.Surname),
                            new XElement("Username", user.Username),
                            new XElement("Cellphone", user.Cellphone)
                        );

                        _xDocument = XDocument.Load(xmlFilePath);
                        _xDocument.Root.Add(userElement);
                    }
                    else
                    {
                        XElement userElement = new XElement("User",
                            new XElement("Id", 1),
                            new XElement("Name", user.Name),
                            new XElement("Surname", user.Surname),
                            new XElement("Username", user.Username),
                            new XElement("Cellphone", user.Cellphone)
                        );

                        _xDocument = new XDocument(new XElement("Users", userElement));
                    }

                    _xDocument.Save(xmlFilePath);

                    TempData.Clear();
                    return RedirectToAction("Index");
                }

                
                TempData["ErrorMessage"] = "Model state in not valid.";
                return View(user);
            }
            catch(Exception ex)
            {                
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }
        }

        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
        {

            XDocument xmlDoc = XDocument.Load("users.xml");

            XElement targetRecord = xmlDoc.Descendants("User").FirstOrDefault(record => (int)record.Element("Id") == id);


            // Edit the record if it exists
            if (targetRecord != null)
            {               

                CreateUserModel user = new CreateUserModel() 
                {
                    Name = targetRecord.Element("Name").Value,
                    Surname = targetRecord.Element("Surname").Value,
                    Username = targetRecord.Element("Username").Value,
                    Cellphone = targetRecord.Element("Cellphone").Value
                };

                TempData.Clear();
                return View(user);
            }
            else
            {                
                TempData["ErrorMessage"] = "Record to edit could not be found.";
                return View();
            }
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, CreateUserModel user, IFormCollection collection)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Surname) || string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.Cellphone))
                {
                    TempData["ErrorMessage"] = "Make sure all fields are filled and not empty.";
                    return View(user);
                }
                if (!CommonFunctions.IsValidPhoneNumber(user.Cellphone))
                {
                    TempData["ErrorMessage"] = "Invalid Cellphone number.";
                    return View(user);
                }

                XDocument xmlDoc = XDocument.Load("users.xml");

                XElement targetRecord = xmlDoc.Descendants("User").FirstOrDefault(record => (int)record.Element("Id") == id);

                // Edit the record if it exists
                if (targetRecord != null)
                {
                    targetRecord.Element("Name").Value = user.Name;
                    targetRecord.Element("Surname").Value = user.Surname;
                    targetRecord.Element("Username").Value = user.Username;
                    targetRecord.Element("Cellphone").Value = user.Cellphone;

                    // Save the updated XML
                    xmlDoc.Save("users.xml");
                    TempData.Clear();
                    return RedirectToAction(nameof(Index));
                }
                else
                {                    
                    TempData["ErrorMessage"] = "Record to edit could not be found.";
                    return View();
                }
                
            }
            catch(Exception ex)
            {                
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            XDocument xmlDoc = XDocument.Load("users.xml");

            XElement targetRecord = xmlDoc.Descendants("User").FirstOrDefault(record => (int)record.Element("Id") == id);


            // Delete the record if it exists
            if (targetRecord != null)
            {

                User user = new User()
                {
                    Id = targetRecord.Element("Id").Value,
                    Name = targetRecord.Element("Name").Value,
                    Surname = targetRecord.Element("Surname").Value,
                    Username = targetRecord.Element("Username").Value,
                    Cellphone = targetRecord.Element("Cellphone").Value
                };

                TempData.Clear();
                return View(user);
            }
            else
            {                
                TempData["ErrorMessage"] = "Record to delete could not be found.";
                return View();
            }
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                XDocument xmlDoc = XDocument.Load("users.xml");

                XElement targetRecord = xmlDoc.Descendants("User").FirstOrDefault(record => (int)record.Element("Id") == id);

                
                if (targetRecord != null)
                {
                    targetRecord.Remove();                    
                    xmlDoc.Save("users.xml");

                    TempData.Clear();
                    return RedirectToAction(nameof(Index));
                }
                else
                {                    
                    TempData["ErrorMessage"] = "Record to delete could not be found.";
                    return View();
                }
                
            }
            catch(Exception ex)
            {                
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }
        }
    }
}
