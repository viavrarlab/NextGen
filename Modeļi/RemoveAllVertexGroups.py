import bpy

D = bpy.data
C = bpy.context
OP = bpy.ops.object

all_objects = []

def add_object_to_array(obj):
    all_objects.append(obj) # Adds passed object to the all object array

def remove_vertex_groups():
    for obj in all_objects:
        for group in obj.vertex_groups:
            obj.vertex_groups.remove(group)

def main():
    for collection in D.collections:    # Loops over all Collections in scene (it's suggested to have only 1 collection to prevent errors)

        for obj in collection.objects:  # Loop over all objects in a collection
            add_object_to_array(obj)    # Function call: Add current object to all objects array
    remove_vertex_groups()

if __name__=="__main__":
    main()