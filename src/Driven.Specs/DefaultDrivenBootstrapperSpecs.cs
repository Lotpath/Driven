using System;
using Driven.Bootstrapper;
using SubSpec;
using Xunit;

namespace Driven.Specs
{
    public class DefaultDrivenBootstrapperSpecs
    {
        [Specification]
        public void can_initialize_bootstrapper()
        {
            var bootstrapper = default(IDrivenBootstrapper);
            var exception = default(Exception);

            "Given a default driven bootstrapper".Context(() =>
                {
                    bootstrapper = new DefaultDrivenBootstrapper();
                });

            "when initializing"
                .Do(() =>
                    {
                        bootstrapper.Initialize();
                    }, (Exception ex) => { exception = ex; });

            "then an exception is not thrown"
                .Assert(() =>
                    {
                        Assert.Null(exception);
                    });
        }

        [Specification]
        public void can_get_engine_for_bootstrapper()
        {
            var bootstrapper = default(IDrivenBootstrapper);
            var engine = default(IDrivenEngine);
            var exception = default(Exception);

            "Given an initialized default driven bootstrapper".Context(() =>
            {
                bootstrapper = new DefaultDrivenBootstrapper();
                bootstrapper.Initialize();
            });

            "when getting a driven engine"
                .Do(() =>
                    {
                        engine = bootstrapper.GetEngine();
                    }, (Exception ex) => { exception = ex; });

            "then an exception is not thrown"
                .Assert(() =>
                {
                    Assert.Null(exception);
                });

            "then an engine is created"
                .Assert(() =>
                {
                    Assert.NotNull(engine);
                });
        }
    }
}