# Kubernetes Secret Scan

## Workflow

There are 3 completly differents Workflow

### Integral Repository Scan

This is an "On Demand" way :
* The user goes on the `Dashboard` (or use the `cli`) and either 
    * Create a `Repository Scan Request` on a Uri
    * Create a `Repository Scan Request` on a known `Repository`
* Request `Status` is marked `New`
* The `Repository Scan Controller` poll frequently the `Queue` for `New Request`
    * Picks up `New Request` and change status to `Validating` (avoid the `Request` to be either lost, or pick twice)
    * Check if the `Repository` is known
        * Create the `Repository` if unknown
    * Associate the `Repository` to the `Request`
    * Request `Status` is changed `Queued`
* The `Repository Scan Scheduler` poll frequently API Server for `WaitingForScheduling`
    * Dequeue `Repository Scan Request` in `Queued` Status
    * Mark as `Scheduling`
    * Check if there are `Repository Scan Agent` available
    * `Assign/Create` an `Repository Scan Agent` to the request
    * Mark `Status` as `Scheduled`
* The `Repository Scan Agent` will
    * Mark `Request Status` to `InProgress`
    * get all the commit history
        * history: `git rev-list --all --pretty=""`
        * only the hash: `git rev-list --all --pretty="" | sed 's/^commit //'`
    * Create a `CommitBatch Scan Request`
        * Mark the `Request` as `New`
        * Associated it with the `Repository`
* The `CommitBatch Scan Controller`
    * Picks up `New Request` and change status to `Validating` 
    * Associate the `Repository` to the `request`
    * Filter out already `Scanned`
    * 
    * Mark `Commit` from the `CommitBatch` as `WaitingForScan` in `Repository History` 
    * Request `Status` is marked `WaitingForScheduling`
* The `CommitBatch Scan Scheduler` will
    * Check if there are `CommitBatch Scan Agent` available
    * `Assign/Create` an `CommitBatch Scan Agent` to the request
    * Mark `Status` as `Scheduled`


### Scheduled Repository Scan
This idea of this flow is to get a way to be sure some repository are tracked down
* A `Repository` is created
* A `Scheduled Repository Scan Request` is created, with the `Repository` attached
    * Contains a `Schedule`
* On every `Run`
    * List of all commit in history
    * Compare with `Scanned Commit`
    * Alert if there are `Unknown Commit` ?
    * Create a `CommitBatch Scan Request` containing the `commit list diff`
    * Queue the `Request`

### Reactive Scan
The idea of these flow is to integrate with the CI in order to :
* Have a fast feedback loop
* Avoid as much as possible to scan an enormous list of commit
* Potentialy block leakage before it happen

#### Scan before Commit
Git Pre-Commit hook
* Very agressive
* Only 1 commit thought (should/must very fast)
* Make sure nothing is leaked on the history
* Can be a source of sadness to developpers ... "per commit"
* Only works if the scan can be done "offline" (on dev computer)

#### Scan before Push
Git Pre-Push hook
* Same as `Pre-commit` but for all the commits push
* a bit less sad than `Pre-commit` hook
* still require to work "offline"
* must find a way to get the list of commits being pushed
    * what about multi branch push ?

#### Scan after Push
Post push (web)hook triggered by github
* Less secure since it's "too late"
* Less distrupting for developpers

