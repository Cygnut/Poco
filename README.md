# Poco
It's a **Po**ularity **Co**ntest!

Website and backend tools which allow users to vote on a heterogeneous database of objects based on popularity.

# API

### GET /getTopScoredEntities

| Parameter | Type | Description |
| --------- | ---- | ----------- |
| offset | Query | The position in the scoretable to start fetching from. Defaults to 0. |
| limit | Query | The number of scoretable entries to fetch. Defaults to 10. |

Fetch a quantity of scoretable entities.

### GET /getVotableEntities

Get a pair of entities that you should vote on. The pair, by design, obey the following:
* Always differ in category (hence 'heterogenous' database).
* Have a 'close' score.

### GET /searchEntities

| Parameter | Type | Description |
| --------- | ---- | ----------- |
| fragment | Query | A string to match against entity information. |
| offset | Query | The position in the set of matching entities to start fetching from. Defaults to 0. |
| limit | Query | The number of matching entities to fetch. Defaults to 10. |

Search for all entities that match a fragment of text.

### POST /voteEntity

| Parameter | Type | Description |
| --------- | ---- | ----------- |
| - | Body | A JSON vote blob. |

Vote on a particular entity. The blob is of the following format: { id, direction }.
