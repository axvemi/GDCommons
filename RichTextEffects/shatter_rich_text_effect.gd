extends RichTextEffect
class_name ShatterTextRichEffect

const bbcode = "shatter"
const p_starting_x_velocity : String = "starting_x_velocity"
const p_starting_y_velocity : String = "starting_y_velocity"
const p_x_velocity_offset : String = "x_velocity_offset"
const p_y_velocity_offset : String = "y_velocity_offset"

const internal_speed : float = 0.002

var rng = RandomNumberGenerator.new()
var gravity_2d : float = ProjectSettings.get_setting("physics/2d/default_gravity")
var char_velocity_dictionary : Dictionary
	

##Take in acount that the change applies to each letter. If we wanted the whole word, update it ONCE per loop
func _process_custom_fx(char_fx):
	if(char_fx.elapsed_time == 0):
		char_velocity_dictionary.clear()
		
	#Init the velocity and direction of every character
	if(!char_velocity_dictionary.has(char_fx.relative_index)):
		var x_direction : int = -1 if rng.randi_range(0, 1) == 0 else 1
		var starting_y_velocity = char_fx.env.get(p_starting_y_velocity, -50)
		var starting_x_velocity = char_fx.env.get(p_starting_x_velocity, -5)
		var x_velocity_offset = char_fx.env.get(p_x_velocity_offset, 50)
		var y_velocity_offset = char_fx.env.get(p_y_velocity_offset, 5)
		
		starting_x_velocity += rng.randf_range(0, x_velocity_offset)
		starting_y_velocity += rng.randf_range(0, y_velocity_offset)
		char_velocity_dictionary[char_fx.relative_index] = Vector2(starting_x_velocity * x_direction, starting_y_velocity)

	var velocity = char_velocity_dictionary[char_fx.relative_index] 
	velocity += Vector2(0, gravity_2d * char_fx.elapsed_time * internal_speed)
	char_fx.offset += velocity * char_fx.elapsed_time
	char_velocity_dictionary[char_fx.relative_index] = velocity
	return true
