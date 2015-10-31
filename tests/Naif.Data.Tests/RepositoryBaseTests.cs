﻿//******************************************
//  Copyright (C) 2014-2015 Charles Nurse  *
//                                         *
//  Licensed under MIT License             *
//  (see included LICENSE)                 *
//                                         *
// *****************************************

using System;
using System.Collections.Generic;
using Moq;
using Moq.Protected;
using Naif.Core.Caching;
using Naif.Core.Collections;
using Naif.TestUtilities;
using Naif.TestUtilities.Fakes;
using Naif.TestUtilities.Models;
using NUnit.Framework;

namespace Naif.Data.Tests
{
    [TestFixture]
    public class RepositoryBaseTests
    {
        // ReSharper disable InconsistentNaming
        private Mock<ICacheProvider> _mockCache;


        [SetUp]
        public void SetUp()
        {
            _mockCache = new Mock<ICacheProvider>();
        }

        [Test]
        public void RepositoryBase_Constructor_Sets_CacheKey_Empty_If_Not_Cacheable()
        {
            //Arrange

            //Act
            var repo = new FakeRepository<Dog>(_mockCache.Object);

            //Assert
            Assert.AreEqual(String.Empty, repo.CacheKey);
        }

        [Test]
        public void RepositoryBase_Constructor_Sets_CacheKey_If_Cacheable()
        {
            //Arrange

            //Act
            var repo = new FakeRepository<CacheableDog>(_mockCache.Object);

            //Assert
            Assert.AreEqual(TestConstants.CACHE_DogsKey, repo.CacheKey);
        }

        [Test]
        public void RepositoryBase_Constructor_Sets_IsCacheable_False_If_Not_Cacheable()
        {
            //Arrange

            //Act
            var repo = new FakeRepository<Dog>(_mockCache.Object);

            //Assert
            Assert.IsFalse(repo.IsCacheable);
        }

        [Test]
        public void RepositoryBase_Constructor_Sets_IsCacheable_True_If_Cacheable()
        {
            //Arrange

            //Act
            var repo = new FakeRepository<CacheableDog>(_mockCache.Object);

            //Assert
            Assert.IsTrue(repo.IsCacheable);
        }

        [Test]
        public void RepositoryBase_Constructor_Sets_IsScoped_False_If_Not_Scoped()
        {
            //Arrange

            //Act
            var repo = new FakeRepository<Dog>(_mockCache.Object);

            //Assert
            Assert.IsFalse(repo.IsScoped);
        }

        [Test]
        public void RepositoryBase_Constructor_Sets_IsScoped_False_If_Cacheable_And_Not_Scoped()
        {
            //Arrange

            //Act
            var repo = new FakeRepository<CacheableDog>(_mockCache.Object);

            //Assert
            Assert.IsFalse(repo.IsScoped);
        }

        [Test]
        public void RepositoryBase_Constructor_Sets_IsScoped_True_If_Scoped()
        {
            //Arrange

            //Act
            var repo = new FakeRepository<Cat>(_mockCache.Object);

            //Assert
            Assert.IsTrue(repo.IsScoped);
        }

        [Test]
        public void RepositoryBase_Constructor_Sets_IsScoped_True_If_Cacheable_And_Scoped()
        {
            //Arrange

            //Act
            var repo = new FakeRepository<CacheableCat>(_mockCache.Object);

            //Assert
            Assert.IsTrue(repo.IsScoped);
        }

        [Test]
        public void RepositoryBase_Constructor_Sets_Scope_Empty_If_Not_Scoped()
        {
            //Arrange

            //Act
            var repo = new FakeRepository<Dog>(_mockCache.Object);

            //Assert
            Assert.AreEqual(String.Empty, repo.Scope);
        }

        [Test]
        public void RepositoryBase_Constructor_Sets_Scope_Empty_If_Cacheable_And_Not_Scoped()
        {
            //Arrange

            //Act
            var repo = new FakeRepository<CacheableDog>(_mockCache.Object);

            //Assert
            Assert.AreEqual(String.Empty, repo.Scope);
        }

        [Test]
        public void RepositoryBase_Constructor_Sets_Scope_If_Scoped()
        {
            //Arrange

            //Act
            var repo = new FakeRepository<Cat>(_mockCache.Object);

            //Assert
            Assert.AreEqual(TestConstants.CACHE_ScopeModule, repo.Scope);
        }

        [Test]
        public void RepositoryBase_Constructor_Sets_Scope_If_Cacheable_And_Scoped()
        {
            //Arrange

            //Act
            var repo = new FakeRepository<CacheableCat>(_mockCache.Object);

            //Assert
            Assert.AreEqual(TestConstants.CACHE_ScopeModule, repo.Scope);
        }

        [Test]
        public void RepositoryBase_Add_Clears_Cache_If_Cacheable()
        {
            //Arrange
            var cacheKey = TestConstants.CACHE_DogsKey;

            _mockCache.Setup(c => c.Get(cacheKey)).Returns(new List<CacheableDog>());

            var mockRepository = new Mock<RepositoryBase<CacheableDog>>(_mockCache.Object);

            //Act
            mockRepository.Object.Add(new CacheableDog());

            //Assert
            _mockCache.Verify(c => c.Remove(cacheKey), Times.Once());
        }

        [Test]
        public void RepositoryBase_Add_Clears_Cache_If_Cacheable_And_Scoped()
        {
            //Arrange
            var cacheKey = String.Format(TestConstants.CACHE_CatsKey + "_" + TestConstants.CACHE_ScopeModule + "_{0}", TestConstants.MODULE_ValidId);

            _mockCache.Setup(c => c.Get(cacheKey)).Returns(new List<CacheableCat>());

            var mockRepository = new Mock<RepositoryBase<CacheableCat>>(_mockCache.Object);

            //Act
            mockRepository.Object.Add(new CacheableCat { ModuleId = TestConstants.MODULE_ValidId });

            //Assert
            _mockCache.Verify(c => c.Remove(cacheKey), Times.Once());
        }

        [Test]
        public void RepositoryBase_Add_Does_Not_Clear_Cache_If_Not_Cacheable()
        {
            //Arrange
            var mockRepository = new Mock<RepositoryBase<Dog>>(_mockCache.Object);

            //Act
            mockRepository.Object.Add(new Dog());

            //Assert
            _mockCache.Verify(c => c.Remove(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void RepositoryBase_Add_Calls_AddInternal()
        {
            //Arrange
            var mockRepository = new Mock<RepositoryBase<Dog>>(_mockCache.Object);
            mockRepository.Protected().Setup("AddInternal", ItExpr.IsAny<Dog>());

            //Act
            mockRepository.Object.Add(new Dog());

            //Assert
            mockRepository.Protected().Verify("AddInternal", Times.Once(), ItExpr.IsAny<Dog>());
        }

        [Test]
        public void RepositoryBase_Delete_Clears_Cache_If_Cacheable()
        {
            //Arrange
            var cacheKey = TestConstants.CACHE_DogsKey;

            _mockCache.Setup(c => c.Get(cacheKey)).Returns(new List<CacheableDog>());

            var mockRepository = new Mock<RepositoryBase<CacheableDog>>(_mockCache.Object);

            //Act
            mockRepository.Object.Delete(new CacheableDog());

            //Assert
            _mockCache.Verify(c => c.Remove(cacheKey), Times.Once());
        }

        [Test]
        public void RepositoryBase_Delete_Clears_Cache_If_Cacheable_And_Scoped()
        {
            //Arrange
            var cacheKey = String.Format(TestConstants.CACHE_CatsKey + "_" + TestConstants.CACHE_ScopeModule + "_{0}", TestConstants.MODULE_ValidId);

            _mockCache.Setup(c => c.Get(cacheKey)).Returns(new List<CacheableCat>());

            var mockRepository = new Mock<RepositoryBase<CacheableCat>>(_mockCache.Object);

            //Act
            mockRepository.Object.Delete(new CacheableCat { ModuleId = TestConstants.MODULE_ValidId });

            //Assert
            _mockCache.Verify(c => c.Remove(cacheKey), Times.Once());
        }

        [Test]
        public void RepositoryBase_Delete_Does_Not_Clear_Cache_If_Not_Cacheable()
        {
            //Arrange
            var mockRepository = new Mock<RepositoryBase<Dog>>(_mockCache.Object);

            //Act
            mockRepository.Object.Delete(new Dog());

            //Assert
            _mockCache.Verify(c => c.Remove(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void RepositoryBase_Delete_Calls_DeleteInternal()
        {
            //Arrange
            var mockRepository = new Mock<RepositoryBase<Dog>>(_mockCache.Object);
            mockRepository.Protected().Setup("DeleteInternal", ItExpr.IsAny<Dog>());

            //Act
            mockRepository.Object.Delete(new Dog());

            //Assert
            mockRepository.Protected().Verify("DeleteInternal", Times.Once(), ItExpr.IsAny<Dog>());
        }

        [Test]
        public void RepositoryBase_GetAll_Checks_Cache_If_Cacheable()
        {
            //Arrange
            var cacheKey = TestConstants.CACHE_DogsKey;

            _mockCache.Setup(c => c.Get(cacheKey)).Returns(new List<CacheableDog>());

            var mockRepository = new Mock<RepositoryBase<CacheableDog>>(_mockCache.Object);

            //Act
            var list = mockRepository.Object.GetAll();

            //Assert
            _mockCache.Verify(c => c.Get(cacheKey));
        }

        [Test]
        public void RepositoryBase_GetAll_Does_Not_Check_Cache_If_Not_Cacheable()
        {
            //Arrange
            var cacheKey = TestConstants.CACHE_DogsKey;
            var mockRepository = new Mock<RepositoryBase<Dog>>(_mockCache.Object);

            //Act
            var list = mockRepository.Object.GetAll();

            //Assert
            _mockCache.Verify(c => c.Get(cacheKey), Times.Never());
        }

        [Test]
        public void RepositoryBase_GetAll_Does_Not_Check_Cache_If_Cacheable_But_Not_Scoped()
        {
            //Arrange
            var cacheKey = TestConstants.CACHE_DogsKey;
            var mockRepository = new Mock<RepositoryBase<CacheableCat>>(_mockCache.Object);

            //Act
            var list = mockRepository.Object.GetAll();

            //Assert
            _mockCache.Verify(c => c.Get(cacheKey), Times.Never());
        }

        [Test]
        public void RepositoryBase_GetAll_Calls_GetAllInternal_If_Cacheable_And_Cache_Expired()
        {
            //Arrange
            var cacheKey = TestConstants.CACHE_DogsKey;
            _mockCache.Setup(c => c.Get(cacheKey)).Returns(null);

            var mockRepository = new Mock<RepositoryBase<CacheableDog>>(_mockCache.Object);
            mockRepository.Protected().Setup("GetAllInternal");

            //Act
            var list = mockRepository.Object.GetAll();

            //Assert
            mockRepository.Protected().Verify("GetAllInternal", Times.Once());
        }

        [Test]
        public void RepositoryBase_GetAll_Calls_GetAllInternal_If_Not_Cacheable()
        {
            //Arrange
            var mockRepository = new Mock<RepositoryBase<Dog>>(_mockCache.Object);
            mockRepository.Protected().Setup("GetAllInternal");

            //Act
            var list = mockRepository.Object.GetAll();

            //Assert
            mockRepository.Protected().Verify("GetAllInternal", Times.Once());
        }

        [Test]
        public void RepositoryBase_GetAll_Calls_GetAllInternal_If_Cacheable_But_Not_Scoped()
        {
            //Arrange
            var mockRepository = new Mock<RepositoryBase<CacheableCat>>(_mockCache.Object);
            mockRepository.Protected().Setup("GetAllInternal");

            //Act
            var list = mockRepository.Object.GetAll();

            //Assert
            mockRepository.Protected().Verify("GetAllInternal", Times.Once());
        }

        [Test]
        public void RepositoryBase_GetAll_Does_Not_Call_GetAllInternal_If_Cacheable_And_Cache_Valid()
        {
            //Arrange
            var cacheKey = TestConstants.CACHE_DogsKey;
            _mockCache.Setup(c => c.Get(cacheKey)).Returns(new List<CacheableDog>());

            var mockRepository = new Mock<RepositoryBase<CacheableDog>>(_mockCache.Object);
            mockRepository.Protected().Setup("GetAllInternal");

            //Act
            var list = mockRepository.Object.GetAll();

            //Assert
            mockRepository.Protected().Verify("GetAllInternal", Times.Never());
        }

        [Test]
        public void RepositoryBase_Get_Checks_Cache_If_Cacheable_And_Scoped()
        {
            //Arrange
            var cacheKey = String.Format(TestConstants.CACHE_CatsKey + "_" + TestConstants.CACHE_ScopeModule + "_{0}", TestConstants.MODULE_ValidId);

            _mockCache.Setup(c => c.Get(cacheKey)).Returns(new List<CacheableCat>());

            var mockRepository = new Mock<RepositoryBase<CacheableCat>>(_mockCache.Object);

            //Act
            var list = mockRepository.Object.Get<int>(TestConstants.MODULE_ValidId);

            //Assert
            _mockCache.Verify(c => c.Get(cacheKey));
        }

        [Test]
        public void RepositoryBase_Get_Throws_If_Not_Cacheable()
        {
            //Arrange
            var mockRepository = new Mock<RepositoryBase<Dog>>(_mockCache.Object);

            //Act, Assert
            Assert.Throws<NotSupportedException>(() => mockRepository.Object.Get<int>(TestConstants.MODULE_ValidId));
        }

        [Test]
        public void RepositoryBase_Get_Throws_If_Not_Scoped()
        {
            //Arrange
            var mockRepository = new Mock<RepositoryBase<Dog>>(_mockCache.Object);

            //Act, Assert
            Assert.Throws<NotSupportedException>(() => mockRepository.Object.Get<int>(TestConstants.MODULE_ValidId));
        }

        [Test]
        public void RepositoryBase_Get_Throws_If_Cacheable_But_Not_Scoped()
        {
            //Arrange
            var mockRepository = new Mock<RepositoryBase<CacheableDog>>(_mockCache.Object);

            //Act, Assert
            Assert.Throws<NotSupportedException>(() => mockRepository.Object.Get<int>(TestConstants.MODULE_ValidId));
        }

        [Test]
        public void RepositoryBase_Get_Calls_GetByScopeInternal_If_Not_Cacheable_And_Is_Scoped()
        {
            //Arrange
            var mockRepository = new Mock<RepositoryBase<Cat>>(_mockCache.Object);
            mockRepository.Protected().Setup<IEnumerable<Cat>>("GetByScopeInternal", ItExpr.IsAny<object>());

            //Act
            var list = mockRepository.Object.Get<int>(TestConstants.MODULE_ValidId);

            //Assert
            mockRepository.Protected().Verify<IEnumerable<Cat>>("GetByScopeInternal", Times.Once(), ItExpr.IsAny<object>());
        }

        [Test]
        public void RepositoryBase_Get_Calls_GetByScopeInternal_If_Cacheable_And_Cache_Expired()
        {
            //Arrange
            var cacheKey = String.Format(TestConstants.CACHE_CatsKey + "_" + TestConstants.CACHE_ScopeModule + "_{0}", TestConstants.MODULE_ValidId);

            _mockCache.Setup(c => c.Get(cacheKey)).Returns(null);

            var mockRepository = new Mock<RepositoryBase<CacheableCat>>(_mockCache.Object);
            mockRepository.Protected().Setup<IEnumerable<CacheableCat>>("GetByScopeInternal", ItExpr.IsAny<object>());

            //Act
            var list = mockRepository.Object.Get<int>(TestConstants.MODULE_ValidId);

            //Assert
            mockRepository.Protected().Verify<IEnumerable<CacheableCat>>("GetByScopeInternal", Times.Once(), ItExpr.IsAny<object>());
        }

        [Test]
        public void RepositoryBase_Get_Does_Not_Call_GetByScopeInternal_If_Cacheable_And_Cache_Valid()
        {
            //Arrange
            var cacheKey = String.Format(TestConstants.CACHE_CatsKey + "_" + TestConstants.CACHE_ScopeModule + "_{0}", TestConstants.MODULE_ValidId);

            _mockCache.Setup(c => c.Get(cacheKey)).Returns(new List<CacheableCat>());

            var mockRepository = new Mock<RepositoryBase<CacheableCat>>(_mockCache.Object);
            mockRepository.Protected().Setup<IEnumerable<CacheableCat>>("GetByScopeInternal", ItExpr.IsAny<object>());

            //Act
            var list = mockRepository.Object.Get<int>(TestConstants.MODULE_ValidId);

            //Assert
            mockRepository.Protected().Verify<IEnumerable<CacheableCat>>("GetByScopeInternal", Times.Never(), ItExpr.IsAny<object>());
        }

        [Test]
        public void RepositoryBase_GetById_Checks_Cache_If_Cacheable()
        {
            //Arrange
            var cacheKey = TestConstants.CACHE_DogsKey;
            _mockCache.Setup(c => c.Get(cacheKey)).Returns(new List<CacheableDog>());

            var mockRepository = new Mock<RepositoryBase<CacheableDog>>(_mockCache.Object);

            //Act
            var dog = mockRepository.Object.GetById(TestConstants.PETAPOCO_ValidDogId);

            //Assert
            _mockCache.Verify(c => c.Get(cacheKey));
        }

        [Test]
        public void RepositoryBase_GetById_Does_Not_Check_Cache_If_Not_Cacheable()
        {
            //Arrange
            var cacheKey = TestConstants.CACHE_DogsKey;
            var mockRepository = new Mock<RepositoryBase<Dog>>(_mockCache.Object);

            //Act
            var dog = mockRepository.Object.GetById(TestConstants.PETAPOCO_ValidDogId);

            //Assert
            _mockCache.Verify(c => c.Get(cacheKey), Times.Never());
        }

        [Test]
        public void RepositoryBase_GetById_Does_Not_Check_Cache_If_Cacheable_But_Not_Scoped()
        {
            //Arrange
            var cacheKey = TestConstants.CACHE_CatsKey;
            var mockRepository = new Mock<RepositoryBase<CacheableCat>>(_mockCache.Object);

            //Act
            var cat = mockRepository.Object.GetById(TestConstants.CACHE_ValidCatId);

            //Assert
            _mockCache.Verify(c => c.Get(cacheKey), Times.Never());
        }

        [Test]
        public void RepositoryBase_GetById_Calls_GetByIdInternal_If_Not_Cacheable()
        {
            //Arrange
            var mockRepository = new Mock<RepositoryBase<Dog>>(_mockCache.Object);
            mockRepository.Protected().Setup<Dog>("GetByIdInternal", ItExpr.IsAny<object>());

            //Act
            var dog = mockRepository.Object.GetById(TestConstants.PETAPOCO_ValidDogId);

            //Assert
            mockRepository.Protected().Verify<Dog>("GetByIdInternal", Times.Once(), ItExpr.IsAny<object>());
        }

        [Test]
        public void RepositoryBase_GetById_Calls_GetByIdInternal_If_Cacheable_But_Not_Scoped()
        {
            //Arrange
            var mockRepository = new Mock<RepositoryBase<CacheableCat>>(_mockCache.Object);
            mockRepository.Protected().Setup<CacheableCat>("GetByIdInternal", ItExpr.IsAny<object>());

            //Act
            var cat = mockRepository.Object.GetById(TestConstants.CACHE_ValidCatId);

            //Assert
            mockRepository.Protected().Verify<CacheableCat>("GetByIdInternal", Times.Once(), ItExpr.IsAny<object>());
        }

        [Test]
        public void RepositoryBase_GetById_Does_Not_Call_GetByIdInternal_If_Cacheable_And_Cache_Valid()
        {
            //Arrange
            var cacheKey = TestConstants.CACHE_DogsKey;
            _mockCache.Setup(c => c.Get(cacheKey)).Returns(new List<CacheableDog>());

            var mockRepository = new Mock<RepositoryBase<CacheableDog>>(_mockCache.Object);
            mockRepository.Protected().Setup<CacheableDog>("GetByIdInternal", ItExpr.IsAny<object>());

            //Act
            var dog = mockRepository.Object.GetById(TestConstants.PETAPOCO_ValidDogId);

            //Assert
            mockRepository.Protected().Verify<CacheableDog>("GetByIdInternal", Times.Never(), ItExpr.IsAny<object>());
        }

        [Test]
        public void RepositoryBase_GetById_Overload_Checks_Cache_If_Cacheable_And_Scoped()
        {
            //Arrange
            var cacheKey = String.Format(TestConstants.CACHE_CatsKey + "_" + TestConstants.CACHE_ScopeModule + "_{0}", TestConstants.MODULE_ValidId);

            _mockCache.Setup(c => c.Get(cacheKey)).Returns(new List<CacheableCat>());

            var mockRepository = new Mock<RepositoryBase<CacheableCat>>(_mockCache.Object);

            //Act
            var cat = mockRepository.Object.GetById(TestConstants.CACHE_ValidCatId, TestConstants.MODULE_ValidId);

            //Assert
            _mockCache.Verify(c => c.Get(cacheKey));
        }

        [Test]
        public void RepositoryBase_GetById_Overload_Throws_If_Not_Cacheable()
        {
            //Arrange
            var mockRepository = new Mock<RepositoryBase<Dog>>(_mockCache.Object);

            //Act, Assert
            Assert.Throws<NotSupportedException>(() => mockRepository.Object.GetById(TestConstants.PETAPOCO_ValidDogId, TestConstants.MODULE_ValidId));
        }

        [Test]
        public void RepositoryBase_GetById_Overload_Throws__If_Cacheable_But_Not_Scoped()
        {
            //Arrange
            var mockRepository = new Mock<RepositoryBase<CacheableDog>>(_mockCache.Object);

            //Act, Assert
            Assert.Throws<NotSupportedException>(() => mockRepository.Object.GetById(TestConstants.PETAPOCO_ValidDogId, TestConstants.MODULE_ValidId));
        }

        [Test]
        public void RepositoryBase_GetPage_Checks_Cache_If_Cacheable()
        {
            //Arrange
            var cacheKey = TestConstants.CACHE_DogsKey;
            _mockCache.Setup(c => c.Get(cacheKey)).Returns(new List<CacheableDog>());

            var mockRepository = new Mock<RepositoryBase<CacheableDog>>(_mockCache.Object);

            //Act
            var dogs = mockRepository.Object.GetPage(TestConstants.PAGE_First, TestConstants.PAGE_RecordCount);

            //Assert
            _mockCache.Verify(c => c.Get(cacheKey));
        }

        [Test]
        public void RepositoryBase_GetPage_Does_Not_Check_Cache_If_Not_Cacheable()
        {
            //Arrange
            var cacheKey = TestConstants.CACHE_DogsKey;
            var mockRepository = new Mock<RepositoryBase<Dog>>(_mockCache.Object);

            //Act
            var dogs = mockRepository.Object.GetPage(TestConstants.PAGE_First, TestConstants.PAGE_RecordCount);

            //Assert
            _mockCache.Verify(c => c.Get(cacheKey), Times.Never());
        }

        [Test]
        public void RepositoryBase_GetPage_Does_Not_Check_Cache_If_Cacheable_But_Not_Scoped()
        {
            //Arrange
            var cacheKey = TestConstants.CACHE_DogsKey;
            var mockRepository = new Mock<RepositoryBase<CacheableCat>>(_mockCache.Object);

            //Act
            var cats = mockRepository.Object.GetPage(TestConstants.PAGE_First, TestConstants.PAGE_RecordCount);

            //Assert
            _mockCache.Verify(c => c.Get(cacheKey), Times.Never());
        }

        [Test]
        public void RepositoryBase_GetPage_Calls_GetPageInternal_If_Not_Cacheable()
        {
            //Arrange
            var mockRepository = new Mock<RepositoryBase<Dog>>(_mockCache.Object);
            mockRepository.Protected().Setup<IPagedList<Dog>>("GetPageInternal", ItExpr.IsAny<int>(), ItExpr.IsAny<int>());

            //Act
            var dogs = mockRepository.Object.GetPage(TestConstants.PAGE_First, TestConstants.PAGE_RecordCount);

            //Assert
            mockRepository.Protected().Verify<IPagedList<Dog>>("GetPageInternal", Times.Once(), ItExpr.IsAny<int>(), ItExpr.IsAny<int>());
        }

        [Test]
        public void RepositoryBase_GetPage_Calls_GetPageInternal_If_Cacheable_But_Not_Scoped()
        {
            //Arrange
            var mockRepository = new Mock<RepositoryBase<CacheableCat>>(_mockCache.Object);
            mockRepository.Protected().Setup<IPagedList<CacheableCat>>("GetPageInternal", ItExpr.IsAny<int>(), ItExpr.IsAny<int>());

            //Act
            var cats = mockRepository.Object.GetPage(TestConstants.PAGE_First, TestConstants.PAGE_RecordCount);

            //Assert
            mockRepository.Protected().Verify<IPagedList<CacheableCat>>("GetPageInternal", Times.Once(), ItExpr.IsAny<int>(), ItExpr.IsAny<int>());
        }

        [Test]
        public void RepositoryBase_GetPage_Does_Not_Call_GetPageInternal_If_Cacheable_And_Cache_Valid()
        {
            //Arrange
            var cacheKey = TestConstants.CACHE_DogsKey;
            _mockCache.Setup(c => c.Get(cacheKey))
                        .Returns(new List<CacheableDog>() { new CacheableDog() });

            var mockRepository = new Mock<RepositoryBase<CacheableDog>>(_mockCache.Object);
            mockRepository.Protected().Setup<IPagedList<CacheableDog>>("GetPageInternal", ItExpr.IsAny<int>(), ItExpr.IsAny<int>());

            //Act
            var dogs = mockRepository.Object.GetPage(TestConstants.PAGE_First, TestConstants.PAGE_RecordCount);

            //Assert
            mockRepository.Protected().Verify<IPagedList<CacheableDog>>("GetPageInternal", Times.Never(), ItExpr.IsAny<int>(), ItExpr.IsAny<int>());
        }

        [Test]
        public void RepositoryBase_GetPage_Overload_Checks_Cache_If_Cacheable_And_Scoped()
        {
            //Arrange
            var cacheKey = String.Format(TestConstants.CACHE_CatsKey + "_" + TestConstants.CACHE_ScopeModule + "_{0}", TestConstants.MODULE_ValidId);

            _mockCache.Setup(c => c.Get(cacheKey)).Returns(new List<CacheableCat>());

            var mockRepository = new Mock<RepositoryBase<CacheableCat>>(_mockCache.Object);

            //Act
            var cats = mockRepository.Object.GetPage<int>(TestConstants.MODULE_ValidId, TestConstants.PAGE_First, TestConstants.PAGE_RecordCount);

            //Assert
            _mockCache.Verify(c => c.Get(cacheKey));
        }

        [Test]
        public void RepositoryBase_GetPage_Overload_Throws_If_Not_Cacheable()
        {
            //Arrange
            var mockRepository = new Mock<RepositoryBase<Dog>>(_mockCache.Object);

            //Act, Assert
            Assert.Throws<NotSupportedException>(() => mockRepository.Object.GetPage<int>(TestConstants.MODULE_ValidId, TestConstants.PAGE_First, TestConstants.PAGE_RecordCount));
        }

        [Test]
        public void RepositoryBase_GetPage_Overload_Throws_If_Not_Scoped()
        {
            //Arrange
            var mockRepository = new Mock<RepositoryBase<Dog>>(_mockCache.Object);

            //Act, Assert
            Assert.Throws<NotSupportedException>(() => mockRepository.Object.GetPage<int>(TestConstants.MODULE_ValidId, TestConstants.PAGE_First, TestConstants.PAGE_RecordCount));
        }

        [Test]
        public void RepositoryBase_GetPage_Overload_Throws_If_Cacheable_But_Not_Scoped()
        {
            //Arrange
            var mockRepository = new Mock<RepositoryBase<CacheableDog>>(_mockCache.Object);

            //Act, Assert
            Assert.Throws<NotSupportedException>(() => mockRepository.Object.GetPage<int>(TestConstants.MODULE_ValidId, TestConstants.PAGE_First, TestConstants.PAGE_RecordCount));
        }

        [Test]
        public void RepositoryBase_GetPage_Overload_Calls_GetPageByScopeInternal_If_Not_Cacheable_And_Is_Scoped()
        {
            //Arrange
            var mockRepository = new Mock<RepositoryBase<Cat>>(_mockCache.Object);
            mockRepository.Protected().Setup<IEnumerable<Cat>>("GetPageByScopeInternal", ItExpr.IsAny<object>(), ItExpr.IsAny<int>(), ItExpr.IsAny<int>());

            //Act
            var cats = mockRepository.Object.GetPage<int>(TestConstants.MODULE_ValidId, TestConstants.PAGE_First, TestConstants.PAGE_RecordCount);

            //Assert
            mockRepository.Protected().Verify<IEnumerable<Cat>>("GetPageByScopeInternal", Times.Once(), ItExpr.IsAny<object>(), ItExpr.IsAny<int>(), ItExpr.IsAny<int>());
        }

        [Test]
        public void RepositoryBase_GetPage_Overload_Calls_GetByScopeInternal_If_Cacheable_And_Cache_Expired()
        {
            //Arrange
            var cacheKey = String.Format(TestConstants.CACHE_CatsKey + "_" + TestConstants.CACHE_ScopeModule + "_{0}", TestConstants.MODULE_ValidId);

            _mockCache.Setup(c => c.Get(cacheKey)).Returns(null);

            var mockRepository = new Mock<RepositoryBase<CacheableCat>>(_mockCache.Object);
            mockRepository.Protected().Setup<IEnumerable<CacheableCat>>("GetByScopeInternal", ItExpr.IsAny<object>())
                                    .Returns(new List<CacheableCat>());

            //Act
            var cats = mockRepository.Object.GetPage<int>(TestConstants.MODULE_ValidId, TestConstants.PAGE_First, TestConstants.PAGE_RecordCount);

            //Assert
            mockRepository.Protected().Verify<IEnumerable<CacheableCat>>("GetByScopeInternal", Times.Once(), ItExpr.IsAny<object>());
        }

        [Test]
        public void RepositoryBase_GetPage_Overload_Does_Not_Call_GetByScopeInternal_If_Cacheable_And_Cache_Valid()
        {
            //Arrange
            var cacheKey = String.Format(TestConstants.CACHE_CatsKey + "_" + TestConstants.CACHE_ScopeModule + "_{0}", TestConstants.MODULE_ValidId);

            _mockCache.Setup(c => c.Get(cacheKey)).Returns(new List<CacheableCat>());

            var mockRepository = new Mock<RepositoryBase<CacheableCat>>(_mockCache.Object);
            mockRepository.Protected().Setup<IEnumerable<CacheableCat>>("GetByScopeInternal", ItExpr.IsAny<object>());

            //Act
            var cats = mockRepository.Object.GetPage<int>(TestConstants.MODULE_ValidId, TestConstants.PAGE_First, TestConstants.PAGE_RecordCount);

            //Assert
            mockRepository.Protected().Verify<IEnumerable<CacheableCat>>("GetByScopeInternal", Times.Never(), ItExpr.IsAny<object>());
        }

        [Test]
        public void RepositoryBase_Update_Clears_Cache_If_Cacheable()
        {
            //Arrange
            var cacheKey = TestConstants.CACHE_DogsKey;

            _mockCache.Setup(c => c.Get(cacheKey)).Returns(new List<CacheableDog>());

            var mockRepository = new Mock<RepositoryBase<CacheableDog>>(_mockCache.Object);

            //Act
            mockRepository.Object.Update(new CacheableDog());

            //Assert
            _mockCache.Verify(c => c.Remove(cacheKey), Times.Once());
        }

        [Test]
        public void RepositoryBase_Update_Clears_Cache_If_Cacheable_And_Scoped()
        {
            //Arrange
            var cacheKey = String.Format(TestConstants.CACHE_CatsKey + "_" + TestConstants.CACHE_ScopeModule + "_{0}", TestConstants.MODULE_ValidId);

            _mockCache.Setup(c => c.Get(cacheKey)).Returns(new List<CacheableCat>());

            var mockRepository = new Mock<RepositoryBase<CacheableCat>>(_mockCache.Object);

            //Act
            mockRepository.Object.Update(new CacheableCat { ModuleId = TestConstants.MODULE_ValidId });

            //Assert
            _mockCache.Verify(c => c.Remove(cacheKey), Times.Once());
        }

        [Test]
        public void RepositoryBase_Update_Does_Not_Clear_Cache_If_Not_Cacheable()
        {
            //Arrange
            var mockRepository = new Mock<RepositoryBase<Dog>>(_mockCache.Object);

            //Act
            mockRepository.Object.Update(new Dog());

            //Assert
            _mockCache.Verify(c => c.Remove(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void RepositoryBase_Update_Calls_UpdateInternal()
        {
            //Arrange
            var mockRepository = new Mock<RepositoryBase<Dog>>(_mockCache.Object);
            mockRepository.Protected().Setup("UpdateInternal", ItExpr.IsAny<Dog>());

            //Act
            mockRepository.Object.Update(new Dog());

            //Assert
            mockRepository.Protected().Verify("UpdateInternal", Times.Once(), ItExpr.IsAny<Dog>());
        }
        // ReSharper restore InconsistentNaming
    }
}
