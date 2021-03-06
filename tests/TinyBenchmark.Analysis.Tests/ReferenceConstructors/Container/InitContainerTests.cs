﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using TinyBenchmark.Analysis;
using TinyBenchmark.Attributes;

namespace Analysis.ReferenceConstructors.Container
{
    /// <summary>
    /// InitContainer can be identified with:
    /// - attribute: InitContainer
    /// - name: InitContainer
    /// - name: Init{class name}
    /// </summary>
    [TestFixture]
    public class InitContainerReferenceTests
    {
        [Test]
        public void ContainerWithoutInitIsAllowed()
        {
            var refConstructor = new ContainerReferenceConstructor(useConventions: true);
            var initContainer = refConstructor.TryCreateInitContainerReference(typeof(ContainerWithoutInit));
            Assert.That(initContainer, Is.Null);
        }

        [Test]
        public void InitContainerAttributeIsFound()
        {
            var refConstructor = new ContainerReferenceConstructor(useConventions: true);
            var initRef = refConstructor.TryCreateInitContainerReference(typeof(ContainerWithInitAttribute));
            Assert.That(initRef?.Method, Is.Not.Null);
            Assert.That(initRef.Method.Name, Is.EqualTo(nameof(ContainerWithInitAttribute.TheContainerInitializationMethod)));
        }

        [Test]
        public void DefaultConventionIsFound()
        {
            var refConstructor = new ContainerReferenceConstructor(useConventions: true);
            var initRef = refConstructor.TryCreateInitContainerReference(typeof(ContainerWithDefaultInitConvention));
            Assert.That(initRef?.Method, Is.Not.Null);
            Assert.That(initRef.Method.Name, Is.EqualTo(nameof(ContainerWithDefaultInitConvention.InitContainer)));
        }

        [Test]
        public void DefaultConventionIsNotFoundBySetup()
        {
            var refConstructor = new ContainerReferenceConstructor(useConventions: false);
            var initRef = refConstructor.TryCreateInitContainerReference(typeof(ContainerWithDefaultInitConvention));
            Assert.That(initRef?.Method, Is.Null);
        }

        [Test]
        public void NamedConventionIsFound()
        {
            var refConstructor = new ContainerReferenceConstructor(useConventions: true);
            var initRef = refConstructor.TryCreateInitContainerReference(typeof(ContainerWithNamedInitConvention));
            Assert.That(initRef?.Method, Is.Not.Null);
            Assert.That(initRef.Method.Name, Is.EqualTo(nameof(ContainerWithNamedInitConvention.InitContainerWithNamedInitConvention)));
        }

        [Test]
        public void NamedConventionIsNotFoundBySetup()
        {
            var refConstructor = new ContainerReferenceConstructor(useConventions: false);
            var initRef = refConstructor.TryCreateInitContainerReference(typeof(ContainerWithNamedInitConvention));
            Assert.That(initRef?.Method, Is.Null);
        }

        [Test]
        public void InitContainerCannotHaveParameters()
        {
            var refConstructor = new ContainerReferenceConstructor(useConventions: true);

            Assert.That(() =>
                refConstructor.TryCreateInitContainerReference(typeof(ContainerWithInitAttributeWithParameters)), Throws.Exception);

            Assert.That(() =>
                refConstructor.TryCreateInitContainerReference(typeof(ContainerWithDefaultInitConventionWithParameters)), Throws.Exception);

            Assert.That(() =>
                refConstructor.TryCreateInitContainerReference(typeof(ContainerWithNamedInitConventionWithParameters)), Throws.Exception);
        }

        [Test]
        public void InitContainerAttributeHasPriorityOverConventions()
        {
            var refConstructor = new ContainerReferenceConstructor(useConventions: true);
            var initRef = refConstructor.TryCreateInitContainerReference(typeof(ContainerWithBothAttributeAndConventions));
            Assert.That(initRef?.Method, Is.Not.Null);
            Assert.That(initRef.Method.Name, Is.EqualTo(nameof(ContainerWithBothAttributeAndConventions.TheContainerInitializationMethod)));
        }

        [Test]
        public void OnlyOneInitContainerIsAllowed()
        {
            var refConstructor = new ContainerReferenceConstructor(useConventions: true);

            Assert.That(() =>
                refConstructor.TryCreateInitContainerReference(typeof(ContainerWithMultipleAttributes)), Throws.Exception);

            Assert.That(() =>
                refConstructor.TryCreateInitContainerReference(typeof(ContainerWithMultipleConventions)), Throws.Exception);
        }
    }

    #region Test types

    internal class ContainerWithoutInit { }

    internal class ContainerWithInitAttribute
    {
        [InitContainer]
        public void TheContainerInitializationMethod() { }
    }

    internal class ContainerWithDefaultInitConvention
    {
        public void InitContainer() { }
    }

    internal class ContainerWithNamedInitConvention
    {
        public void InitContainerWithNamedInitConvention() { }
    }

    #region Init method with parameters

    internal class ContainerWithInitAttributeWithParameters
    {
        [InitContainer]
        public void TheContainerInitializationMethod(int num) { }
    }

    internal class ContainerWithDefaultInitConventionWithParameters
    {
        public void InitContainer(int num) { }
    }

    internal class ContainerWithNamedInitConventionWithParameters
    {
        public void InitContainerWithNamedInitConventionWithParameters(int num) { }
    }

    #endregion

    #region Multiple init methods

    internal class ContainerWithBothAttributeAndConventions
    {
        [InitContainer]
        public void TheContainerInitializationMethod() { }

        public void InitContainer() { }

        public void InitContainerWithBothAttributeAndConventions() { }
    }

    internal class ContainerWithMultipleAttributes
    {
        [InitContainer]
        public void TheContainerInitializationMethod() { }

        [InitContainer]
        public void AnotherContainerInitializationMethod() { }
    }

    internal class ContainerWithMultipleConventions
    {
        public void InitContainer() { }

        public void InitContainerWithMultipleConventions() { }
    }

    #endregion

    #endregion
}
