# ReverseProxy.RuntimeModel namespace

Classes in this folder define the internal representation
of ReverseProxy's runtime state used in perf-critical code paths.

All classes should be immutable, and all members and members of members
MUST be either:

   A) immutable
   B) `AtomicHolder<T>` wrapping an immutable type `T`.
   C) Thread-safe (e.g. `AtomicCounter`)

This ensures we can easily handle hot-swappable configurations
without explicit synchronization overhead across threads,
and each thread can operate safely with up-to-date yet consistent information
(always the latest and consistent snapshot available when processing of a request starts).

## Class naming conventions

* Classes named `*Info` (`RouteInfo`, `ClusterInfo`, `EndpointInfo`)
  represent the 3 primary abstractions in Reverse Proxy (Routes, Clusters and Destinations);

* Classes named `*Config` (`RouteConfig`, `ClusterConfig`, `EndpointConfig`)
  represent portions of the 3 abstractions that only change in reaction to 
  Reverse Proxy config changes.
  For example, when the health check interval for a cluster is updated,
  a new instance of `ClusterConfig` is created with the new values,
  and the corresponding `AtomicHolder` in `ClusterInfo` is updated to point at the new instance;

* Classes named `*DynamicState` (`ClusterDynamicState`, `EndpointDynamicState`)
  represent portions of the 3 abstractions that change in reaction to
  Reverse Proxy's runtime state.
  For example, when new destinations are discovered for a cluster,
  a new instance of `ClusterDynamicState` is created with the new values,
  and the corresponding `AtomicHolder` in `ClusterInfo` is updated to point at the new instance;
