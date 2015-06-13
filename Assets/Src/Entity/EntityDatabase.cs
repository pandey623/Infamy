using System;
using UnityEngine;
using System.Collections.Generic;

public class EntityDatabaseEntry {
    public int enterCount;
    public int exitCount;
    public bool destroyed;
    public bool disabled;
    public int captureCount;
    public FactionId factionId;
    public EntitySize size;
    public EntityType type;
    public Entity entity;

    public EntityDatabaseEntry(Entity entity) {
        this.entity = entity;
        this.enterCount = 0;
        this.exitCount = 0;
        this.disabled = false;
        this.destroyed = false;
        this.factionId = entity.factionId;
        this.size = entity.size;
        this.type = entity.type;
    }

    public bool Active {
        get { return enterCount > 0 & exitCount < enterCount && !destroyed; }
    }
}
/*
    EntityDatabase.GetHostiles(FactionId)
 * EntityDatabase.Query((e) => {
 *    if(e.type == Type.Fighter && e.health > 0.5f && e.piloted) {
 *     
 *    }
 *  var query = new Query();
 *  query.factionIds = {"1", "2"};
 *  EntityDatabase.QueryFaction(Id, () => )
 *  EntityDatabase.QueryActiveFaction(Id, () => )
 *  EntityDatatbase.IsDestroyed("entityId")
 *  EntityDatabase.HasEnteredMission("entityId")
 *  
 * MissionLog.GetRecord("EnteredBattle", "EntityId")
 * }
 */
public static class EntityDatabase {
    public static Dictionary<string, Entity> database;
    public static List<Entity> entities;
    public static List<Entity> ActiveEntities;
    public static List<Entity> InactiveEntities;

    public delegate bool EntityQuery(Entity entity);

    public static List<Entity> Query(EntityQuery query) {
        return null;
    }

    public static void Add(Entity entity) {
        var entry = new EntityDatabaseEntry(entity);
    }
}