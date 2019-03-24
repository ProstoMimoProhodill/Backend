﻿using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using GraphLabs.Backend.Domain;
using GraphLabs.Backend.Domain.VariantData;
using Newtonsoft.Json;

namespace GraphLabs.Backend.DAL
{
    public class InitialData
    {
        private readonly PasswordHashCalculator _calculator;

        public InitialData(PasswordHashCalculator calculator)
        {
            _calculator = calculator;
        }
        
        public IEnumerable<TaskModule> GetTaskModules()
        {
            // taskModule #1
            yield return new TaskModule
            {
                Id = 1,
                Name = "Изоморфизм",
                Description = "Даны два графа. Доказать их изоморфность путём наложения вершин одного графа на вершины другого, или обосновать, почему это невозможно.",
                Version = "2.0"
            };

            // taskModule #2
            yield return  new TaskModule
            {
                Id = 2,
                Name = "Модуль Лизы",
                Description = "Описание",
                Version = "1.0"
            };
        }

        public IEnumerable<TaskVariant> GetTaskVariants(IEnumerable<TaskModule> modules)
        {
            var idCounter = 0;

            var sampleData = new VariantData<Graph>
            {
                Type = VariantDataType.Graph,
                Value = new Graph
                {
                    Vertices = new[] {"1", "2", "3", "4", "5"},
                    Edges = new[]
                    {
                        new Edge {Source = "1", Target = "2"},
                        new Edge {Source = "2", Target = "3"},
                        new Edge {Source = "3", Target = "4"},
                        new Edge {Source = "4", Target = "5"},
                        new Edge {Source = "5", Target = "1"}
                    }
                }
            };
            
            foreach (var taskModule in modules)
            {
                for (var i = 0; i < 3; i++)
                {
                    idCounter++;
                    yield return new TaskVariant
                    {
                        Id = idCounter,
                        Name = $"Вариант {idCounter}",
                        TaskModule = taskModule,
                        VariantData = JsonConvert.SerializeObject(new [] { sampleData })
                    };
                }
            }
        }

        public IEnumerable<User> GetUsers()
        {
            var crypto = new RNGCryptoServiceProvider();
            var salts = Enumerable.Range(0, 10)
                .Select(_ => new byte[16])
                .Select(s =>
                {
                    crypto.GetBytes(s);
                    return s;
                })
                .ToArray();
            
            var idCounter = 0;
            yield return new Teacher
            {
                Id = ++idCounter,
                Email = "admin@graphlabs.ru",
                FirstName = "Администратор",
                LastName = "Администратор",
                FatherName = "Администратор",
                PasswordSalt = salts[idCounter],
                PasswordHash = _calculator.Calculate("admin", salts[idCounter])
            };
            
            yield return new Student
            {
                Id = ++idCounter,
                Email = "student-1@graphlabs.ru",
                FirstName = "Студент Первый Тестовый",
                LastName = "Первый",
                FatherName = "Тестовый",
                Group = "Первая Тестовая",
                PasswordSalt = salts[idCounter],
                PasswordHash = _calculator.Calculate("первый", salts[idCounter])
            };
            
            yield return new Student
            {
                Id = ++idCounter,
                Email = "student-2@graphlabs.ru",
                FirstName = "Студент Второй Тестовый",
                LastName = "Второй",
                FatherName = "Тестовый",
                Group = "Первая Тестовая",
                PasswordSalt = salts[idCounter],
                PasswordHash = _calculator.Calculate("второй", salts[idCounter])
            };
            
            yield return new Student
            {
                Id = ++idCounter,
                Email = "student-3@graphlabs.ru",
                FirstName = "Студент Третий Тестовый",
                LastName = "Третий",
                FatherName = "Тестовый",
                Group = "Вторая Тестовая",
                PasswordSalt = salts[idCounter],
                PasswordHash = _calculator.Calculate("третий", salts[idCounter])
            };
            
            yield return new Student
            {
                Id = ++idCounter,
                Email = "student-4@graphlabs.ru",
                FirstName = "Студент Четвёртый Тестовый",
                LastName = "Четвёртый",
                FatherName = "Тестовый",
                Group = "Вторая Тестовая",
                PasswordSalt = salts[idCounter],
                PasswordHash = _calculator.Calculate("четвёртный", salts[idCounter])
            };
        }
    }
}