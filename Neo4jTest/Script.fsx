#r @"..\packages\Neo4jClient.1.0.0.651\lib\net40\Neo4jClient.dll"
#r @"..\packages\Newtonsoft.Json.5.0.1\lib\net45\Newtonsoft.Json.dll"

open System
open System.Collections.Generic
open Neo4jClient
open Neo4jClient.Cypher

let client = GraphClient(Uri "http://localhost:7474/db/data")
client.Connect()

[<CLIMutable>]
type Person = { Name: string; Age: int }

type PersonRelationship (targetNode) =
    inherit Relationship (targetNode)
    override x.RelationshipTypeKey = "FRIEND"
    interface IRelationshipAllowingSourceNode<Person>
    interface IRelationshipAllowingTargetNode<Person>

let generatePeople n =
    let rnd = Random()
    seq { 1..n }
    |> Seq.map (fun i -> { Name = sprintf "person%d" i; Age = rnd.Next(20, 40) })
    |> Seq.map client.Create
    |> Seq.pairwise
    |> Seq.iter (fun (x, y) -> client.CreateRelationship(x, PersonRelationship(y)) |> ignore)

generatePeople 100

let query = 
    client.Cypher
          //.Match("(zai:Person)-[:LOVES]->(kot:Person)")
          .Match("(p)")
          .Where("p.Name = 'person3'")
          .Return<Person>("p")

let p = query.Results |> Seq.toList