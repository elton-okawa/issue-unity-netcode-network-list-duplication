# Bug
Client-side `NetworkList` has duplicated values from server-side list after a specific scene flow

# Description
Initially, everything syncs correctly but after leaving the `ListScene` and going back, 
it seems that client-side `NetworkList` is synched with values from server and also
receive add `NetworkListEvents` which causes the observed duplication

# Steps to Reproduce

1. Clone this repository
2. Start an editor and a clone using `ParrelSync` menu
3. Start as `host` in the editor
4. Start as `client` in the clone
5. Notice that both values is correctly synched and there are logs showing current `NetworkList` values
6. In `host` window, change the scene by clicking on `DummyScene`
7. In `host` window, go back to the first scene by clicking on `ListScene`
8. Notice that `client` window has duplicated values and its logs shows the same state
