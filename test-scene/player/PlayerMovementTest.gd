class_name PlayerMovementTest
extends GdUnitTestSuite
@warning_ignore('unused_parameter')
@warning_ignore('return_value_discarded')

var runner: GdUnitSceneRunner
var world_environment: Node
var player: Node

func before() -> void:
	runner = scene_runner("res://ui/game_loading_screen/game_loading_scene.tscn")

	var main = runner.invoke("find_child", "Main")
	world_environment = main.get_node("WorldEnvironment")

	await assert_signal(world_environment).is_emitted("EverythingLoaded")

	player = world_environment.get_node("PlayerBlue")
	assert_that(player).is_not_null()

func test_is_moved_forward() -> void:
	var initial_position = player.position

	runner.simulate_key_press(KEY_W)
	await await_idle_frame()

	var new_position = player.position
	assert_float(new_position.z).is_less(initial_position.z)

	runner.simulate_key_release(KEY_W)

func test_is_moved_backward() -> void:
	var initial_position = player.position

	runner.simulate_key_press(KEY_S)
	await await_idle_frame()

	var new_position = player.position
	assert_float(new_position.z).is_greater(initial_position.z)

	runner.simulate_key_release(KEY_S)

func test_is_moved_left() -> void:
	var initial_position = player.position

	runner.simulate_key_press(KEY_A)
	await await_idle_frame()

	var new_position = player.position
	assert_float(new_position.x).is_less(initial_position.x)

	runner.simulate_key_release(KEY_A)

func test_is_moved_right() -> void:
	var initial_position = player.position

	runner.simulate_key_press(KEY_D)
	await await_idle_frame()

	var new_position = player.position
	assert_float(new_position.x).is_greater(initial_position.x)

	runner.simulate_key_release(KEY_D)

func test_is_not_moved_diagonally_NE() -> void:
	is_not_moved_diagonally(KEY_W, KEY_D)

func test_is_not_moved_diagonally_NW() -> void:
	is_not_moved_diagonally(KEY_W, KEY_A)

func test_is_not_moved_diagonally_SW() -> void:
	is_not_moved_diagonally(KEY_S, KEY_A)

func test_is_not_moved_diagonally_SE() -> void:
	is_not_moved_diagonally(KEY_S, KEY_D)

func is_not_moved_diagonally(key1: Key, key2: Key) -> void:
	var initial_position = player.position

	runner.simulate_key_press(key1)
	runner.simulate_key_press(key2)
	await await_idle_frame()

	var new_position = player.position
	assert_float(new_position.x).is_equal(initial_position.x)

	runner.simulate_key_release(key1)
	runner.simulate_key_release(key2)
