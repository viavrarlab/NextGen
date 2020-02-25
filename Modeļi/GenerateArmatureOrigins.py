import bpy
from math import radians
from mathutils import Vector

D = bpy.data
C = bpy.context
OP = bpy.ops.object

all_objects = []    # Array to store all mesh objects in scene


def deselect_all():
    bpy.ops.object.select_all(action='DESELECT')    # Deselects all objects in scene
    
def select_all():
    bpy.ops.object.select_all(action='SELECT')    # Deselects all objects in scene

def set_origin(obj):
    deselect_all()  # Make sure nothing is selected to prevent selection errors for future operations
    obj.select_set(True)    # Select the object
    bpy.ops.object.origin_set(type='ORIGIN_CENTER_OF_MASS', center='MEDIAN') # Set the object's origin to its Center of Mass
    obj.select_set(False)   # Deselect the object

def add_object_to_array(obj):
    all_objects.append(obj) # Adds passed object to the all object array

def create_armature_at_origins():
    bpy.ops.object.add(type='ARMATURE', location=(0.0,0.0,0.0)) # Creates an empty Armature object at 0,0,0 position
    armature = bpy.data.objects['Armature'] # Store Armature object in variable for easy access
    for obj in all_objects: # Loop over all mesh objects to create a bone for each of them
        object_name = obj.name+"_Bone"  # Store current object's name for later use
        C.scene.cursor.location = obj.location  # Set 3D cursor position to current object's origin location
        cursor = C.scene.cursor.location # Store 3D cursor's location for later use
        C.view_layer.objects.active = armature  # Set Armature object as active to perform bone adding and positioning actions
        bpy.ops.object.editmode_toggle()    # Toggle into Edit mode for Aramture
        b = armature.data.edit_bones.new(object_name)   # Create a new bone inside Armature
        b.head = cursor # Position the new bone's head end at 3D cursor
        b.tail = (cursor.x+0.0, cursor.y+0.0, cursor.z+1.0) # Position the new bone's tail end at offset from 3D cursor
        bpy.ops.object.editmode_toggle() # Toggle out of Edit mode
        
def fix_object_rotation():
    deselect_all()
    select_all()
    bpy.ops.transform.rotate(value=radians(-90), orient_axis='X', orient_type='GLOBAL', orient_matrix=((1, 0, 0), (0, 1, 0), (0, 0, 1)), orient_matrix_type='GLOBAL')
    bpy.ops.object.transform_apply(location=False, rotation=True, scale=False)
    bpy.ops.transform.rotate(value=radians(90), orient_axis='X', orient_type='GLOBAL', orient_matrix=((1, 0, 0), (0, 1, 0), (0, 0, 1)), orient_matrix_type='GLOBAL')
        
        
def rig():
    armature = bpy.data.objects['Armature'] # Store Armature object in variable for later use
    for obj in all_objects: # Loop over all mesh objects to set parent to the Armature
        deselect_all()  # Make sure nothing is selected to prevent selection errors for future operations
                
        armature.select_set(True) # Select Armature
        
        OP.mode_set(mode='EDIT')    # Go into Edit mode for Armature
        parent_bone = obj.name+"_Bone"  # Get and store current object's name to use to find the correct bone for parenting
        armature.data.edit_bones.active = armature.data.edit_bones[parent_bone] # Set previously determined bone as selected 
        OP.mode_set(mode='OBJECT') # Go into Object mode for Armature
        deselect_all() # Make sure nothing is selected to prevent selection errors for future operations
                
        obj.select_set(True)    # Select mesh object as first, making it the object to be parented
        armature.select_set(True) # Select the Armature as second, making it the parent object
        bpy.ops.object.parent_set(type='ARMATURE_NAME')  # Parent the selected objects using the active/selected bone type (BONE)
        
        vg = obj.vertex_groups[parent_bone]
        verts = []
        for vert in obj.data.vertices:
            verts.append(vert.index)
        vg.add(verts, 1.0, 'ADD')

def main():
    for collection in D.collections:    # Loops over all Collections in scene (it's suggested to have only 1 collection to prevent errors)

        for obj in collection.objects:  # Loop over all objects in a collection
            set_origin(obj) # Function call: Set current objects origin
            add_object_to_array(obj)    # Function call: Add current object to all objects array
        
        fix_object_rotation()
        
        create_armature_at_origins()    # Function call: Create an Armature and its bones at each mesh objects origin
        rig()   # Function call: Parent all mesh objects to Armature

if __name__=="__main__":
    main()

