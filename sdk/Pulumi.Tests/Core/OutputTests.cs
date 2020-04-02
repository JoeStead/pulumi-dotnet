﻿// Copyright 2016-2019, Pulumi Corporation

using System.Collections.Immutable;
using System.Threading.Tasks;
using Pulumi.Serialization;
using Xunit;

namespace Pulumi.Tests.Core
{
    public partial class OutputTests : PulumiTest
    {
        private static Output<T> CreateOutput<T>(T value, bool isKnown, bool isSecret = false)
            => new Output<T>(Task.FromResult(OutputData.Create(
                ImmutableHashSet<Resource>.Empty, value, isKnown, isSecret)));

        public class PreviewTests
        {
            [Fact]
            public Task ApplyCanRunOnKnownValue()
                => RunInPreview(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: true);
                    var o2 = o1.Apply(a => a + 1);
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.True(data.IsKnown);
                    Assert.Equal(1, data.Value);
                });

            [Fact]
            public Task ApplyCanRunOnKnownAwaitableValue()
                => RunInPreview(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: true);
                    var o2 = o1.Apply(a => Task.FromResult("inner"));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.True(data.IsKnown);
                    Assert.Equal("inner", data.Value);
                });

            [Fact]
            public Task ApplyCanRunOnKnownKnownOutputValue()
                => RunInPreview(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: true);
                    var o2 = o1.Apply(a => CreateOutput("inner", isKnown: true));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.True(data.IsKnown);
                    Assert.Equal("inner", data.Value);
                });

            [Fact]
            public Task ApplyCanRunOnKnownUnknownOutputValue()
                => RunInPreview(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: true);
                    var o2 = o1.Apply(a => CreateOutput("inner", isKnown: false));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.Equal("inner", data.Value);
                });

            [Fact]
            public Task ApplyProducesUnknownDefaultOnUnknown()
                => RunInPreview(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: false);
                    var o2 = o1.Apply(a => a + 1);
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.Equal(0, data.Value);
                });

            [Fact]
            public Task ApplyProducesUnknownDefaultOnUnknownAwaitable()
                => RunInPreview(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: false);
                    var o2 = o1.Apply(a => Task.FromResult("inner"));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.Null(data.Value);
                });

            [Fact]
            public Task ApplyProducesUnknownDefaultOnUnknownKnownOutput()
                => RunInPreview(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: false);
                    var o2 = o1.Apply(a => CreateOutput("", isKnown: true));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.Null(data.Value);
                });

            [Fact]
            public Task ApplyProducesUnknownDefaultOnUnknownUnknownOutput()
                => RunInPreview(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: false);
                    var o2 = o1.Apply(a => CreateOutput("", isKnown: false));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.Null(data.Value);
                });

            [Fact]
            public Task ApplyPreservesSecretOnKnown()
                => RunInPreview(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: true, isSecret: true);
                    var o2 = o1.Apply(a => a + 1);
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.True(data.IsKnown);
                    Assert.True(data.IsSecret);
                    Assert.Equal(1, data.Value);
                });

            [Fact]
            public Task ApplyPreservesSecretOnKnownAwaitable()
                => RunInPreview(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: true, isSecret: true);
                    var o2 = o1.Apply(a => Task.FromResult("inner"));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.True(data.IsKnown);
                    Assert.True(data.IsSecret);
                    Assert.Equal("inner", data.Value);
                });

            [Fact]
            public Task ApplyPreservesSecretOnKnownKnownOutput()
                => RunInPreview(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: true, isSecret: true);
                    var o2 = o1.Apply(a => CreateOutput("inner", isKnown: true));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.True(data.IsKnown);
                    Assert.True(data.IsSecret);
                    Assert.Equal("inner", data.Value);
                });

            [Fact]
            public Task ApplyPreservesSecretOnKnownUnknownOutput()
                => RunInPreview(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: true, isSecret: true);
                    var o2 = o1.Apply(a => CreateOutput("inner", isKnown: false));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.True(data.IsSecret);
                    Assert.Equal("inner", data.Value);
                });

            [Fact]
            public Task ApplyPreservesSecretOnUnknown()
                => RunInPreview(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: false, isSecret: true);
                    var o2 = o1.Apply(a => a + 1);
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.True(data.IsSecret);
                    Assert.Equal(0, data.Value);
                });

            [Fact]
            public Task ApplyPreservesSecretOnUnknownAwaitable()
                => RunInPreview(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: false, isSecret: true);
                    var o2 = o1.Apply(a => Task.FromResult("inner"));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.True(data.IsSecret);
                    Assert.Null(data.Value);
                });

            [Fact]
            public Task ApplyPreservesSecretOnUnknownKnownOutput()
                => RunInPreview(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: false, isSecret: true);
                    var o2 = o1.Apply(a => CreateOutput("inner", isKnown: true));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.True(data.IsSecret);
                    Assert.Null(data.Value);
                });

            [Fact]
            public Task ApplyPreservesSecretOnUnknownUnknownOutput()
                => RunInPreview(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: false, isSecret: true);
                    var o2 = o1.Apply(a => CreateOutput("inner", isKnown: false));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.True(data.IsSecret);
                    Assert.Null(data.Value);
                });

            [Fact]
            public Task ApplyPropagatesSecretOnKnownKnownOutput()
                => RunInPreview(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: true);
                    var o2 = o1.Apply(a => CreateOutput("inner", isKnown: true, isSecret: true));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.True(data.IsKnown);
                    Assert.True(data.IsSecret);
                    Assert.Equal("inner", data.Value);
                });

            [Fact]
            public Task ApplyPropagatesSecretOnKnownUnknownOutput()
                => RunInPreview(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: true);
                    var o2 = o1.Apply(a => CreateOutput("inner", isKnown: false, isSecret: true));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.True(data.IsSecret);
                    Assert.Equal("inner", data.Value);
                });

            [Fact]
            public Task ApplyDoesNotPropagateSecretOnUnknownKnownOutput()
                => RunInPreview(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: false);
                    var o2 = o1.Apply(a => CreateOutput("inner", isKnown: true, isSecret: true));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.False(data.IsSecret);
                    Assert.Null(data.Value);
                });

            [Fact]
            public Task ApplyDoesNotPropagateSecretOnUnknownUnknownOutput()
                => RunInPreview(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: false);
                    var o2 = o1.Apply(a => CreateOutput("inner", isKnown: false, isSecret: true));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.False(data.IsSecret);
                    Assert.Null(data.Value);
                });
        }

        public class NormalTests
        {
            [Fact]
            public Task ApplyCanRunOnKnownValue()
                => RunInNormal(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: true);
                    var o2 = o1.Apply(a => a + 1);
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.True(data.IsKnown);
                    Assert.Equal(1, data.Value);
                });

            [Fact]
            public Task ApplyCanRunOnKnownAwaitableValue()
                => RunInNormal(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: true);
                    var o2 = o1.Apply(a => Task.FromResult("inner"));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.True(data.IsKnown);
                    Assert.Equal("inner", data.Value);
                });

            [Fact]
            public Task ApplyCanRunOnKnownKnownOutputValue()
                => RunInNormal(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: true);
                    var o2 = o1.Apply(a => CreateOutput("inner", isKnown: true));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.True(data.IsKnown);
                    Assert.Equal("inner", data.Value);
                });

            [Fact]
            public Task ApplyCanRunOnKnownUnknownOutputValue()
                => RunInNormal(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: true);
                    var o2 = o1.Apply(a => CreateOutput("inner", isKnown: false));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.Equal("inner", data.Value);
                });

            [Fact]
            public Task ApplyProducesKnownOnUnknown()
                => RunInNormal(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: false);
                    var o2 = o1.Apply(a => a + 1);
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.Equal(1, data.Value);
                });

            [Fact]
            public Task ApplyProducesKnownOnUnknownAwaitable()
                => RunInNormal(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: false);
                    var o2 = o1.Apply(a => Task.FromResult("inner"));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.Equal("inner", data.Value);
                });

            [Fact]
            public Task ApplyProducesKnownOnUnknownKnownOutput()
                => RunInNormal(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: false);
                    var o2 = o1.Apply(a => CreateOutput("inner", isKnown: true));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.Equal("inner", data.Value);
                });

            [Fact]
            public Task ApplyProducesUnknownOnUnknownUnknownOutput()
                => RunInNormal(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: false);
                    var o2 = o1.Apply(a => CreateOutput("inner", isKnown: false));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.Equal("inner", data.Value);
                });

            [Fact]
            public Task ApplyPreservesSecretOnKnown()
                => RunInNormal(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: true, isSecret: true);
                    var o2 = o1.Apply(a => a + 1);
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.True(data.IsKnown);
                    Assert.True(data.IsSecret);
                    Assert.Equal(1, data.Value);
                });

            [Fact]
            public Task ApplyPreservesSecretOnKnownAwaitable()
                => RunInNormal(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: true, isSecret: true);
                    var o2 = o1.Apply(a => Task.FromResult("inner"));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.True(data.IsKnown);
                    Assert.True(data.IsSecret);
                    Assert.Equal("inner", data.Value);
                });

            [Fact]
            public Task ApplyPreservesSecretOnKnownKnownOutput()
                => RunInNormal(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: true, isSecret: true);
                    var o2 = o1.Apply(a => CreateOutput("inner", isKnown: true));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.True(data.IsKnown);
                    Assert.True(data.IsSecret);
                    Assert.Equal("inner", data.Value);
                });

            [Fact]
            public Task ApplyPreservesSecretOnKnownUnknownOutput()
                => RunInNormal(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: true, isSecret: true);
                    var o2 = o1.Apply(a => CreateOutput("inner", isKnown: false));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.True(data.IsSecret);
                    Assert.Equal("inner", data.Value);
                });

            [Fact]
            public Task ApplyPreservesSecretOnUnknown()
                => RunInNormal(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: false, isSecret: true);
                    var o2 = o1.Apply(a => a + 1);
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.True(data.IsSecret);
                    Assert.Equal(1, data.Value);
                });

            [Fact]
            public Task ApplyPreservesSecretOnUnknownAwaitable()
                => RunInNormal(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: false, isSecret: true);
                    var o2 = o1.Apply(a => Task.FromResult("inner"));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.True(data.IsSecret);
                    Assert.Equal("inner", data.Value);
                });

            [Fact]
            public Task ApplyPreservesSecretOnUnknownKnownOutput()
                => RunInNormal(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: false, isSecret: true);
                    var o2 = o1.Apply(a => CreateOutput("inner", isKnown: true));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.True(data.IsSecret);
                    Assert.Equal("inner", data.Value);
                });

            [Fact]
            public Task ApplyPreservesSecretOnUnknownUnknownOutput()
                => RunInNormal(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: false, isSecret: true);
                    var o2 = o1.Apply(a => CreateOutput("inner", isKnown: false));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.True(data.IsSecret);
                    Assert.Equal("inner", data.Value);
                });

            [Fact]
            public Task ApplyPropagatesSecretOnKnownKnownOutput()
                => RunInNormal(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: true);
                    var o2 = o1.Apply(a => CreateOutput("inner", isKnown: true, isSecret: true));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.True(data.IsKnown);
                    Assert.True(data.IsSecret);
                    Assert.Equal("inner", data.Value);
                });

            [Fact]
            public Task ApplyPropagatesSecretOnKnownUnknownOutput()
                => RunInNormal(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: true);
                    var o2 = o1.Apply(a => CreateOutput("inner", isKnown: false, isSecret: true));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.True(data.IsSecret);
                    Assert.Equal("inner", data.Value);
                });

            [Fact]
            public Task ApplyPropagatesSecretOnUnknownKnownOutput()
                => RunInNormal(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: false);
                    var o2 = o1.Apply(a => CreateOutput("inner", isKnown: true, isSecret: true));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.True(data.IsSecret);
                    Assert.Equal("inner", data.Value);
                });

            [Fact]
            public Task ApplyPropagatesSecretOnUnknownUnknownOutput()
                => RunInNormal(async () =>
                {
                    var o1 = CreateOutput(0, isKnown: false);
                    var o2 = o1.Apply(a => CreateOutput("inner", isKnown: false, isSecret: true));
                    var data = await o2.DataTask.ConfigureAwait(false);
                    Assert.False(data.IsKnown);
                    Assert.True(data.IsSecret);
                    Assert.Equal("inner", data.Value);
                });
        }
    }
}
