﻿using System;
using System.Collections.Generic;
using Xunit;
using Log4Pro.DIServices.Caching;
using Log4Pro.DIServices.Caching.Providers.ManagedMemory;
using Log4Pro.DIServices;
using Log4Pro.Classes.UnitTestExtensions;

namespace Log4Pro.Test.DIServices.Caching.Providers
{
	public class ManagedMemoryCacheProvider
	{
		private readonly ManagedMemoryCache _cache = new();

		[Fact(DisplayName = "Implement ICacheProvider interface")]
		public void IsICachePRovider()
		{
			Assert.True(_cache is ICacheProvider);
		}

		[Fact(DisplayName = "Base functions (add, read, remove) works")]
		public void RemoveWork()
		{
			int testValue = 7;
			var testId = GetTestId();
			_cache.Publish(testId, testValue);
			var readed = _cache.Read<int>(testId);
			Assert.Equal(testValue, readed);
			_cache.Remove(testId);
			Assert.Throws<NotFoundException>(() => _cache.Read<int>(testId));
		}

		[Fact(DisplayName = "Read throws NotFoundException when the readed data is not exist.")]
		public void NotFoundException()
		{
			Assert.Throws<NotFoundException>(() => _cache.Read<int>("NonExistDataId"));
		}

		private string GetTestId() => Guid.NewGuid().ToString();
	}
}
