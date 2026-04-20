; Globals
(global object player_main_actor none)

; Externs
(script extern void mp_hide_scoreboard)
(script extern void (give_unit_weapon (unit player) (anytag primary) (anytag secondary)))

; Scripts
(script static void disable_player_control
	(player_enable_input false)
	(player_disable_movement true)
	(player_camera_control false)
)

(script static void enable_player_control
	(player_enable_input true)
	(player_disable_movement false)
	(player_camera_control true)
)

(script static void hide_ui_and_hud
	(chud_cinematic_fade 0 0)
	(chud_show_messages false)
	(sound_class_set_gain "ui" 0 0)
	(sound_class_set_gain "game_event" 0 0)
	(sleep 1)
	(mp_hide_scoreboard)
)

(script static void show_ui_and_hud
	(chud_cinematic_fade 1 0)
	(chud_show_messages true)
	(sound_class_set_gain "ui" 1 0)
	(sound_class_set_gain "game_event" 1 0)
)

(script static void reset
	(enable_player_control)
	(show_ui_and_hud)
	(camera_control false)
	(cinematic_show_letterbox false)
)

(script dormant void client_reset
	(reset)
)

(script static void global_reset
	(mp_wake_script client_reset)
)

(script static void get_main_actor
	(set player_main_actor (player_get 0))
)

(script static void start_movie_clients
	(mp_wake_script movie_start)
)

(script dormant void movie_start
	(disable_player_control)
	(hide_ui_and_hud)
	(fade_in 0 0 0 10000)
	(cinematic_set_title memory_movie_message_0)
	(sleep 300)
	(camera_control true)
	(cinematic_show_letterbox true)
	(get_main_actor)
	(sleep 10)
	(fade_in 0 0 0 50)
	(scene_1)
)

(script static void scene_1
	(sound_class_set_gain "unit_footsteps" 0 0)
	(camera_set_relative "scene0_starting_cam" 0 player_main_actor)
	(object_set_permutation player_main_actor "helmet" "mp_odst")
	(object_set_permutation player_main_actor "leftshoulder" "mp_regulator")
	(object_set_permutation player_main_actor "rightshoulder" "mp_regulator")
	(sleep 100)
	(custom_animation_loop (unit player_main_actor) "objects\characters\masterchief\masterchief" "combat:unarmed:walk_front" true)
	(sleep 10)
	(custom_animation_loop (unit player_main_actor) "objects\characters\masterchief\masterchief" "combat:unarmed:walk_right" true)
	(sleep 10)
	(custom_animation_loop (unit player_main_actor) "objects\characters\masterchief\masterchief" "combat:unarmed:walk_left" true)
	(sleep 10)
	(custom_animation_loop (unit player_main_actor) "objects\characters\masterchief\masterchief" "combat:unarmed:walk_back" true)
	(sleep 10)
	(custom_animation_loop (unit player_main_actor) "objects\characters\masterchief\masterchief" "combat:unarmed:walk_front" true)
)

; Temporary variables for trilateration 
(global real gps_tmp1 0.0)
(global real gps_tmp2 0.0)
(global real gps_tmp3 0.0)
(global real gps_tmp4 0.0)
; Holds the trilaterated coordinates
(global real obj_x 0.0)
(global real obj_y 0.0)
(global real obj_z 0.0)

(script static void (get_obj_pos (object target))
  (set gps_tmp1 (objects_distance_to_flag target "gps1"))
  (set gps_tmp2 (objects_distance_to_flag target "gps2"))
  (set gps_tmp3 (objects_distance_to_flag target "gps3"))
  (set gps_tmp4 (objects_distance_to_flag target "gps4"))
  (set gps_tmp1 (* gps_tmp1 gps_tmp1))
  (set gps_tmp2 (* gps_tmp2 gps_tmp2))
  (set gps_tmp3 (* gps_tmp3 gps_tmp3))
  (set gps_tmp4 (* gps_tmp4 gps_tmp4))
  (set gps_tmp1 (+ gps_tmp1 (* gps_tmp2 -1) 1))
  (set gps_tmp2 (- gps_tmp2 gps_tmp3))
  (set gps_tmp3 (- gps_tmp3 gps_tmp4))
  (set obj_x (/ gps_tmp1 2))
  (set obj_y (/ (+ gps_tmp1 gps_tmp2) 2))
  (set obj_z (/ (+ gps_tmp1 gps_tmp2 gps_tmp3) 2))
)

(script continuous void easter_egg
	(sleep_until
		(and
			(player_action_test_jump)
			(player_action_test_rotate_grenades)
		)
	)
	(player_action_test_reset)
	(effect_new_on_object_marker "objects\characters\grunt\fx\grunt_birthday_party" player_main_actor "head")
)

(script static void fly_test
	(camera_control false)
	(camera_set_relative "origin_cam" 0 "bipd9")
	(custom_animation_loop "bipd9" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:forward" true)
	(sleep 60)
	(custom_animation_loop "bipd9" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:backward" true)
	(sleep 60)
	(custom_animation_loop "bipd9" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:left" true)
	(sleep 60)
	(custom_animation_loop "bipd9" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:right" true)
	(sleep 60)
	(unit_stop_custom_animation "bipd9")
)

(script static void fly_test2
	(custom_animation_loop "bipd9" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:turn_left" true)
)

(script static void aim_test
	(vehicle_load_magic "bipd9" "mongoose_d" (players))
	(player_control_lock_gaze (player_get 0) "cset/p0" 220)
	(sleep 200)
	(custom_animation_loop "bipd9" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:forward" true)
	(sleep 200)
	(unit_stop_custom_animation "bipd9")
	(player_control_unlock_gaze (player_get 0))
)

(script static void seat_test
	(vehicle_load_magic "bipd9" "mongoose_d" (players))
)

;(script static void seat_test2
;	(vehicle_load_magic "vehi9" "mongoose_d" (players))
;)

(script static void turret
	(vehicle_auto_turret "vehi0" "scen0" 3000 1 3000)
)

(script static void look_here
	(player_control_lock_gaze (unit player_main_actor) "cset/p0" 220)
)

(script static void take_this 
	(give_unit_weapon "bipd9" "objects\weapons\rifle\sniper_rifle\sniper_rifle.weapon" "objects\weapons\support_high\rocket_launcher\rocket_launcher.weapon")
)

(script static void take_this2
	(give_unit_weapon (player_get 0) "objects\weapons\rifle\sniper_rifle\sniper_rifle.weapon" "objects\weapons\support_high\rocket_launcher\rocket_launcher.weapon")
)

(script static void bighead
	(custom_animation_loop (player_get 0) "objects\characters\masterchief\masterchief" "any:big_head" true)
)

(script static void weld 
	(objects_attach (list_get (volume_return_objects_by_type "scen0" 128) 0) "" (list_get (volume_return_objects_by_type "scen0" 1) 0) "")
)

(script static void weld2
	(objects_attach (list_get (volume_return_objects_by_type "scen0" 128) 0) "" (list_get (players) 0) "body")
)

(script static void weld3
	(objects_attach (list_get (volume_return_objects_by_type "scen0" 128) 1) "" (list_get (players) 0) "head")
)

(script static void weld4
	(objects_attach (list_get (volume_return_objects_by_type "scen0" 2048) 0) "seat" (list_get (players) 0) "target_main")
)

(script static void weld5
	(objects_attach (list_get (players) 0) "target_main" (list_get (volume_return_objects_by_type "scen0" 2048) 0) "seat")
)

(global object chair none)

;(script static void sit
;	(set player_main_actor (list_get (players) 0))
;	(get_obj_pos player_main_actor)
;	(set chair (list_get (position_return_nearby_objects_by_type obj_x obj_y obj_z 2 512) 0))
;	(objects_attach player_main_actor "target_main" chair "seat")
;)

; (set chair position_return_nearby_objects_by_type x y z radius type)

(script static void sit
	(set player_main_actor (list_get (players) 0))
	(set chair (list_get (volume_return_objects_by_type "scen0" 512) 0))
	(objects_attach chair "seat" player_main_actor "target_main")
)

(script static void overlay
	(custom_animation_loop (player_get 0) "objects\characters\masterchief\masterchief" "combat:unarmed:throw_equipment" true)
)

(script static void overlay2
	(custom_animation_loop (player_get 0) "objects\characters\masterchief\masterchief" "combat:unarmed:walk_front" true)
)

(script static void overlay3
	(custom_animation_loop (player_get 0) "objects\characters\masterchief\masterchief" "behemoth_g:any:grip" true)
)

(script static void wave
	(custom_animation_loop (player_get 0) "objects\characters\masterchief\masterchief" "any:wave_flag" true)
)

(script static void wave2
	(custom_animation_loop (player_get 0) "objects\characters\masterchief\masterchief" "combat:flag:any:wave_flag" true)
)

(script static void fast
	(object_set_custom_animation_speed (list_get (players) 0) 1.25)
)

(script static void slow
	(object_set_custom_animation_speed (list_get (players) 0) 0.75)
)

(script static void go_test
	(vs_enable_targeting "sq0" false)
	(vs_enable_looking "sq0" false)
	(vs_enable_moving "sq0" false)
	(vs_suppress_activity_termination "sq0" true)
	(vs_fly_to "sq0" true "cset/p0")
)

(script static void follow
	(vs_enable_targeting "sq0" false)
	(vs_enable_looking "sq0" false)
	(vs_enable_moving "sq0" false)
	(vs_suppress_activity_termination "sq0" true)
	(vs_approach_player "sq0" true 0.1 500 500)
)

(script static void place
	(ai_place "sq0")
	(sleep 1)
	(vehicle_load_magic (ai_get_object "sq0") "mongoose_d" (players))
)

(script static void cam 
	(camera_control true)
	(camera_set_relative "forward_cam" 0 (ai_get_object "sq0"))
)

(script static void cam2
	(camera_control true)
	(camera_set_first_person (player_get 0))
)

(script static void cam3
	(camera_control true)
	(camera_set_first_person (ai_get_unit "sq0"))
)

(script static void cam4
	(camera_control true)
	(camera_set_animation_relative "objects\eldewrito\forge\misc\aiming_vector_animations" "any:forward" (player_get 0) "front")
)

(script static void tex
	(texture_camera_set_object_marker "bloc29" "origin" 50)
)

(script static void tex2
	(texture_camera_set_object_marker (list_get (players) 0) "visor_reflection" 50)
)

(global object sink none)
(global object sign none)
(global scenery train_car none)

(script static void tex3
	(scenery_animation_start_loop "scen49" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:forward")
	(set sink (list_get (volume_return_objects "scen49") 0))
	(texture_camera_set_object_marker sink "tex_cam" 40)
)

(script static void spin
	(set sign (list_get (volume_return_objects_by_type "scen49" 2048) 0))
	(objects_attach "scen49" "origin" sign "")
	(scenery_animation_start_loop "scen49" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:spin_right")
)

(script static void spin2
	(set sign (list_get (volume_return_objects_by_type "scen49" 2048) 0))
	(objects_attach "scen49" "primary_trigger" sign "")
	(scenery_animation_start_at_frame_loop "scen49" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:spin_right" 0)
)

(script static void train
	(set train_car (list_get (volume_return_objects_by_type "scen0" 128) 0))
	(scenery_animation_start_loop train_car "objects\levels\multi\s3d_turf\turf_train\turf_train" "turf_train" 0)
)

(script static void train2
	(set train_car (list_get (volume_return_objects_by_type "scen0" 128) 1))
	(scenery_animation_start_loop train_car "objects\levels\multi\s3d_turf\turf_train\turf_train" "turf_train" 0)
)

(script static void hornet
	(set train_car (list_get (volume_return_objects_by_type "scen0" 128) 1))
	(scenery_animation_start_loop train_car "objects\levels\multi\s3d_turf\turf_hornet\s3d_turf\s3d_turf" "hornet1" 0)
)

(script static void hornet2
	(scenery_animation_start_loop train_car "objects\levels\multi\s3d_turf\turf_hornet\s3d_turf\s3d_turf" "hornet2" 0)
)

(script static void hornet3
	(scenery_animation_start_loop train_car "objects\levels\multi\s3d_turf\turf_hornet\s3d_turf\s3d_turf" "hornet_test" 0)
)

(global short counter 0)

(script static void rotate_counter_clockwise
	(sleep_until
		(begin
			(object_set_facing (list_get (players) 0) "front")
			(sleep 30)
			(object_set_facing (list_get (players) 0) "left")
			(sleep 30)
			(object_set_facing (list_get (players) 0) "back")
			(sleep 30)
			(object_set_facing (list_get (players) 0) "right")
			(sleep 30)
			(set counter (+ counter 1))
			(if (= counter 1) (print "global is 1"))
			(if (= counter 2) (print "global is 2"))
			(if (= counter 3) (print "global is 3"))
			(> counter 5)
		)
	1)
)

(script static void spell
	(scenery_animation_start_loop "scen49" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:spell_eldewrito")
)

(script static void spell2
	(scenery_animation_start "scen49" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:spell_eldewrito")
)

(script static void spell3
	(object_set_function_variable "scen49" "welding_trail" 1 0)
	(scenery_animation_start "scen49" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:spell_eldewrito")
)

(script static void spell4
	(object_set_function_variable "scen49" "welding_trail" 0 1)
	(scenery_animation_start "scen49" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:spell_eldewrito")
)

(script static void spell5
	(scenery_animation_start "scen49" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:spell_eldewrito")
	(object_set_function_variable "scen49" "welding_trail" 1 0)
	(sleep (scenery_get_animation_time "scen49"))
	(object_clear_function_variable "scen49" "welding_trail")
)

(script static void spell6
	(scenery_animation_start "scen49" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:spell_eldewrito")
	(object_set_function_variable "scen49" "welding_trail" 1 0)
	(sleep 30)
	(object_clear_function_variable "scen49" "welding_trail")
	(sleep 1)
	(object_set_function_variable "scen49" "welding_trail" 1 0)
	(sleep 30)
	(object_clear_function_variable "scen49" "welding_trail")
	(sleep 1)
	(object_set_function_variable "scen49" "welding_trail" 1 0)
	(sleep 30)
	(object_clear_function_variable "scen49" "welding_trail")
	(sleep 1)
	(object_set_function_variable "scen49" "welding_trail" 1 0)
	(sleep 30)
	(object_clear_function_variable "scen49" "welding_trail")
	(sleep 1)
	(object_set_function_variable "scen49" "welding_trail" 1 0)
	(sleep 30)
	(object_clear_function_variable "scen49" "welding_trail")
	(sleep 1)
	(object_set_function_variable "scen49" "welding_trail" 1 0)
	(sleep 30)
	(object_clear_function_variable "scen49" "welding_trail")
	(sleep 1)
	(object_set_function_variable "scen49" "welding_trail" 1 0)
	(sleep 30)
	(object_clear_function_variable "scen49" "welding_trail")
	(sleep 1)
	(object_set_function_variable "scen49" "welding_trail" 1 0)
	(sleep 30)
	(object_clear_function_variable "scen49" "welding_trail")
)

; cleaner, however, the first part gets cut slightly because of execution time it takes to get to the set function on?
(script static void spell7
	(scenery_animation_start "scen49" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:spell_eldewrito")
	(sleep_until
		(begin
			(object_set_function_variable "scen49" "welding_trail" 1 0)
			(sleep 30)
			(object_clear_function_variable "scen49" "welding_trail")
			(sleep 1)
			(< (scenery_get_animation_time "scen49") 1)
		)
	1)
)

; simplify, fixes issue
(script static void spell8
	(scenery_animation_start "scen49" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:spell_eldewrito")
	(object_set_function_variable "scen49" "welding_trail" 1 0)
	(sleep_until
		(begin
			(object_set_function_variable "scen49" "welding_trail" 1 0)
			(sleep 30)
			(object_clear_function_variable "scen49" "welding_trail")
			(< (scenery_get_animation_time "scen49") 1)
		)
	1)
)

(script static void spell9
	(scenery_animation_start "scen49" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:spell_eldewrito")
	(object_set_function_variable "scen49" "welding_laser" 1 0)
	(object_set_function_variable "scen49" "welding_sparks" 1 0)
	(object_set_function_variable "scen49" "welding_trail" 1 0)
	(sleep_until
		(begin
			(object_set_function_variable "scen49" "welding_trail" 1 0)
			(sleep 30)
			(object_clear_function_variable "scen49" "welding_trail")
			(< (scenery_get_animation_time "scen49") 1)
		)
	1)
	(object_clear_function_variable "scen49" "welding_laser")
	(object_clear_function_variable "scen49" "welding_sparks")
)

(script static void smile

	(scenery_animation_start "scen49" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:draw_frowny_face_head")
	(object_set_function_variable "scen49" "fine_point_pen" 1 0)
	(sleep (scenery_get_animation_time "scen49"))
	(object_clear_function_variable "scen49" "fine_point_pen")
	(sleep 5)
	
	(scenery_animation_start "scen49" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:draw_frowny_face_left_eye")
	(object_set_function_variable "scen49" "fine_point_pen" 1 0)
	(sleep (scenery_get_animation_time "scen49"))
	(object_clear_function_variable "scen49" "fine_point_pen")
	(sleep 5)
	
	(scenery_animation_start "scen49" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:draw_frowny_face_right_eye")
	(object_set_function_variable "scen49" "fine_point_pen" 1 0)
	(sleep (scenery_get_animation_time "scen49"))
	(object_clear_function_variable "scen49" "fine_point_pen")
	(sleep 5)
	
	(scenery_animation_start "scen49" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:draw_frowny_face_mouth")
	(object_set_function_variable "scen49" "fine_point_pen" 1 0)
	(sleep (scenery_get_animation_time "scen49"))
	(object_clear_function_variable "scen49" "fine_point_pen")
	(sleep 5)
	
	(scenery_animation_start "scen49" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:draw_frowny_face_question")
	(object_set_function_variable "scen49" "fine_point_pen" 1 0)
	(sleep (scenery_get_animation_time "scen49"))
	(object_clear_function_variable "scen49" "fine_point_pen")
	(sleep 5)
	
	(scenery_animation_start "scen49" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:draw_frowny_face_dot")
	(object_set_function_variable "scen49" "fine_point_pen" 1 0)
	(sleep (scenery_get_animation_time "scen49"))
	(object_clear_function_variable "scen49" "fine_point_pen")
	(sleep 5)

)

(script static void visor
	(object_set_permutation (list_get (players) 0) "helmet" "realtime")
)

; dunno what this is doing if anything, again, we should check stuff in ida ourselves
(script static void look_here0
	(camera_control true)
	(camera_set_target 0 "bloc29")
)

; works
(script static void look_here2
	(camera_control true)
	(camera_set_flying_cam_at_point 0 "scene0_starting_cam")
)

; works
(script static void look_here3
	(camera_control true)
	(camera_set_mode 0 1)
)

; works
(script static void look_here4
	(camera_control true)
	(camera_set_mode 0 2)
)

; works
(script static void look_here5
	(camera_control true)
	(camera_set_mode 0 0)
)

; these work, effects are obvious and not obvious at the same time, need a more detailed definition
(script static void what
	(mover_set_program (list_get (players) 0) 0)
)

(script static void what2
	(mover_set_program (list_get (players) 0) 1)
)

(script static void what3
	(mover_set_program (list_get (players) 0) 2)
)

(script static void what4
	(mover_set_program (list_get (players) 0) 3)
)

(script static void what5
	(mover_set_program (list_get (players) 0) 4)
)

(script static void copy
	(objects_attach (list_get (players) 0) "head" "bipd9" "")
	(sleep 90)
	(objects_detach (list_get (players) 0) "bipd9")
)

; works
(script static void lol
	(mp_game_won mp_team_red)
)

(script static void lol2
	(mp_object_belongs_to_team "vehi9" mp_team_blue)
)

; works
(script static void lol3
	(mp_respawn_override_timers mp_team_red)
)

(script static void lol4
	(mp_allegiance mp_team_red mp_team_blue)
)

(script static void lol5
	(give_medal 5)
	(give_medal 6)
	(give_medal 7)
)

(script static void face_me
	(object_set_facing (list_get (players) 0) "front")
)

(script static void face_me2
	(object_set_facing "bipd9" "front")
)

(script static void face_me3
	(object_set_facing "vehi9" "front")
)

(script static void scnr_func
	(interpolator_start "scenario_interpolator1" 1 30)
)

(script static void scnr_func2
	(interpolator_restart_all)
)

(global real store 0.0)

(script static void scnr_func3
	(set store (interpolator_get_current_value "scenario_interpolator1" true))
	(object_set_velocity (list_get (players) 0) store)
)

; works, 0 to 3 corresponds to a to d
;(script static void global_func
;	(object_function_set 1 1)
;	(object_function_set 3 1)
;	(sleep 90)
;	(object_function_set 1 -1)
;	(object_function_set 3 -1)
;)

; (camera_place_relative) does something, but not in a way that makes sense outside of sapien

; works
;(script static void lol
;	(custom_animation_list (players) "objects\characters\masterchief\masterchief" "combat:unarmed:walk_front" true)
;)

; doesn't work (obvious but still)
;(script static void lol
;	(custom_animation (unit "scen49") "objects\eldewrito\forge\misc\aiming_vector_animations" "any:spell_eldewrito" true)
;)

; works
;(script static void lol
;	(unit_set_active_camo (player_get 0) true 0)
;)

; works
;(script continuous void upright
;	(if (= (vehicle_overturned "vehi9") true)
;		(vehicle_flip "vehi9")
;	)
;)
	
; one second to change to value 0
(script static void off
	(object_set_function_variable "scen49" "welding_trail" 0 1)
)

(script static void off2
	(object_set_function_variable "scen49" "welding_trail" 0.1 0.1)
)

(script static void off3
	(object_clear_function_variable "scen49" "welding_trail")
)

; one second to change to value 1
(script static void on
	(object_set_function_variable "scen49" "welding_trail" 1 1)
)

(script static void why
	(object_set_function_variable "scen49" "welding_trail" 1 1)
	(sleep 90)
	(object_set_function_variable "scen49" "welding_trail" 0 1)
)

(script static void on2
	(object_set_function_variable "scen49" "welding_trail" 0 1)
)

(script static void on3
	(object_set_function_variable "scen49" "welding_trail" 1 0)
)

(script static void on4
	(object_set_function_variable "scen49" "welding_trail" 10 10)
)

(script static void speed
	(object_set_custom_animation_speed "scen49" 0.1)
)

(script static void speed2
	(object_set_custom_animation_speed "scen49" 1)
)

(script static void speed3
	(object_set_custom_animation_speed "scen49" 2)
)

(script static void jmm
	(custom_animation_loop "bipd9" "objects\eldewrito\forge\misc\aiming_vector_animations" "any:down" true)
)

(script static void lens1
	(texture_camera_set_fov 40)
)

(script static void lens2
	(texture_camera_set_fov 50)
)

(script static void lens3
	(texture_camera_set_fov 60)
)

(script static void lens4
	(texture_camera_set_fov 70)
)

(script static void lens5
	(texture_camera_set_fov 180)
)

(script static void hd
	(texture_camera_set_resolution 1000 1000)
)

(script static void hd2
	(texture_camera_set_resolution 2000 2000)
)

(script static void hd3
	(texture_camera_set_resolution 3000 3000)
)

(script static void sd
	(texture_camera_set_resolution 50 50)
)

(script static void sd2
	(texture_camera_set_resolution 100 100)
)

(script static void sd3
	(texture_camera_set_resolution 512 288)
)


; works
(script static void light
	(texture_camera_enable_dynamic_lights true)
)

; works
(script static void op1
	(texture_camera_set_fov 120)
)

; works
(script static void op2
	(texture_camera_set_fov_frame_target 70)
)

(script static void spar
	(mp_ai_place "sq0/sp2")
	(give_unit_weapon (ai_get_unit "sq0/sp2") "objects\weapons\rifle\sniper_rifle\sniper_rifle.weapon" "objects\weapons\support_high\rocket_launcher\rocket_launcher.weapon")
)

(script static void spar2
	(ai_place "sq0/sp2")
	(give_unit_weapon (ai_get_unit "sq0/sp2") "objects\weapons\rifle\sniper_rifle\sniper_rifle.weapon" "objects\weapons\support_high\rocket_launcher\rocket_launcher.weapon")
)

(script static void sparlook
	(vs_suppress_activity_termination "sq0" true)
	(vs_enable_targeting "sq0" false)
	(vs_enable_looking "sq0" false)
	(vs_face "sq0/sp2" true "cset/p0")
	(vs_look "sq0/sp2" true "cset/p0")
)

(script static void sparlook2
	(vs_suppress_activity_termination "sq0" true)
	(vs_enable_targeting "sq0" false)
	(vs_enable_looking "sq0" false)
	(vs_face "sq0/sp2" true "cset/p1")
	(vs_look "sq0/sp2" true "cset/p0")
)

(script static void spargo
	(vs_fly_to "sq0/sp2" false "cset/p1")
	(vs_fly_to "sq0/sp2" true "cset/p0")
)

(script static void spargo2
	(vs_fly_to "sq0/sp2" false "cset/p0")
	(vs_fly_to "sq0/sp2" true "cset/p1")
)

(script static void spargre
	(vs_grenade "sq0/sp2" true "cset/p1" 0)
)

(script static void spargive
	(unit_add_equipment (ai_get_unit "sq0/sp2") "start_assault" true true)
)
	
(script static void elite
	(mp_ai_place "sq0/sp3")
	(give_unit_weapon (ai_get_unit "sq0/sp3") "objects\weapons\support_high\rocket_launcher\rocket_launcher.weapon" "objects\weapons\rifle\sniper_rifle\sniper_rifle.weapon")
)

(script static void elite2
	(ai_place "sq0/sp3")
	(give_unit_weapon (ai_get_unit "sq0/sp3") "objects\weapons\support_high\rocket_launcher\rocket_launcher.weapon" "objects\weapons\rifle\sniper_rifle\sniper_rifle.weapon")
)

(script static void weap
	(give_unit_weapon (ai_get_unit "sq0/sp2") "objects\weapons\rifle\battle_rifle\battle_rifle.weapon" "objects\weapons\rifle\sniper_rifle\sniper_rifle.weapon")
)

(script static void weap2
	(give_unit_weapon (ai_get_unit "sq0/sp2") "objects\weapons\melee\energy_blade\energy_blade.weapon" "objects\weapons\rifle\sniper_rifle\sniper_rifle.weapon")
)

(script static void weap3
	(give_unit_weapon (ai_get_unit "sq0/sp2") "objects\weapons\melee\gravity_hammer\gravity_hammer.weapon" "objects\weapons\rifle\sniper_rifle\sniper_rifle.weapon")
)

(script static void weap4
	(give_unit_weapon (ai_get_unit "sq0/sp2") "objects\weapons\multiplayer\ball\ball.weapon" "objects\weapons\rifle\sniper_rifle\sniper_rifle.weapon")
)

(script static void weap5
	(give_unit_weapon (ai_get_unit "sq0/sp2") "objects\eldewrito\movie\props\eldewrito_flag.weapon" "objects\weapons\rifle\sniper_rifle\sniper_rifle.weapon")
)

(script static void weap6
	(give_unit_weapon (ai_get_unit "sq0/sp2") "objects\weapons\rifle\smg\smg.weapon" "objects\weapons\rifle\sniper_rifle\sniper_rifle.weapon")
)

(script static void weap7
	(give_unit_weapon (ai_get_unit "sq0/sp3") "objects\weapons\melee\gravity_hammer\gravity_hammer.weapon" "objects\weapons\rifle\sniper_rifle\sniper_rifle.weapon")
)

(script static void weap8
	(give_unit_weapon (ai_get_unit "sq0/sp3") "objects\eldewrito\movie\props\eldewrito_flag.weapon" "objects\weapons\rifle\sniper_rifle\sniper_rifle.weapon")
)

(script static void weap9
	(give_unit_weapon (ai_get_unit "sq0/sp3") "objects\weapons\melee\energy_blade\unarmed.weapon" "objects\weapons\melee\energy_blade\unarmed.weapon")
)

(script static void weap10
	(give_unit_weapon (ai_get_unit "sq0/sp2") "objects\weapons\melee\energy_blade\unarmed.weapon" "objects\weapons\melee\energy_blade\unarmed.weapon")
)

(script static void suit
	(object_set_variant (ai_get_object "sq0/sp2") "newbie")
	(sleep 60)
	(object_set_variant (ai_get_object "sq0/sp2") "minelelol")
	(sleep 60)
	(object_set_variant (ai_get_object "sq0/sp2") "zekken")
	(sleep 60)
	(object_set_variant (ai_get_object "sq0/sp2") "tpf")
	(sleep 60)
	(object_set_variant (ai_get_object "sq0/sp2") "liver_k")
	(sleep 60)
	(object_set_variant (ai_get_object "sq0/sp2") "sirius")
	(sleep 60)
	(object_set_variant (ai_get_object "sq0/sp2") "forgot")
	(sleep 60)
	(object_set_variant (ai_get_object "sq0/sp2") "dirkkane")
	(sleep 60)
	(object_set_variant (ai_get_object "sq0/sp2") "canadiator")
	(sleep 60)
	(object_set_variant (ai_get_object "sq0/sp2") "sarge")
	(sleep 60)
	(object_set_variant (ai_get_object "sq0/sp2") "big_guy")
	(sleep 60)
	(object_set_variant (ai_get_object "sq0/sp2") "maroon_maze")
	(sleep 60)
	(object_set_variant (ai_get_object "sq0/sp2") "blade_of_anduril")
	(sleep 60)
	(object_set_variant (ai_get_object "sq0/sp2") "microwave_oven")
	(sleep 60)
	(object_set_variant (ai_get_object "sq0/sp2") "mtndewit")
	(sleep 60)
	(object_set_variant (ai_get_object "sq0/sp2") "twisted_flog")
	(sleep 60)
	(object_set_variant (ai_get_object "sq0/sp2") "duck")
)

(sleep (* 4 30))

(script static void suit2
	(object_set_variant (ai_get_object "sq0/sp2") "default")
)

(script static void suit_dance
	(custom_animation_loop (ai_get_unit "sq0/sp2") "objects\characters\masterchief\masterchief" "combat:any:any:taunt_mad" true)
)

(script static void suit_dance2
	(unit_stop_custom_animation (ai_get_unit "sq0/sp2"))
)

(script static void hide
	(object_function_set 3 1.0)
)

(script static void show
	(object_function_set 3 0.0)
)

(script static void putback
	(object_create_containing "scen")
)

(script static void see
	(ai_set_deaf "sq0" false)
	(ai_set_blind "sq0" false)
)

(script static void see2
	(ai_set_deaf "sq0" true)
	(ai_set_blind "sq0" true)
)

(script static void walker
	(ai_place "sq0/sp1")
	(vs_approach_player "sq0/sp1" true 0.1 500 500)
)

(script static void walker_go
	(ai_place "sq0/sp1")
	(cs_run_command_script "sq0/sp1" course)
)

(script static void walker_go2
	(ai_place "sq0/sp1")
	(cs_run_command_script "sq0/sp1" course2)
)

(script static void walker_go3
	(ai_place "sq0/sp1")
	(cs_run_command_script "sq0/sp1" course3)
)

(script command_script course
	(cs_fly_to "cset/p0")
	(cs_fly_to "cset/p1")
	(cs_fly_to "cset/p2")
	(cs_fly_to "cset/p3")
)

(script command_script course2
	(cs_fly_to "sset/p0")
	(cs_fly_to "sset/p1")
	(cs_fly_to "sset/p2")
	(cs_fly_to "sset/p3")
)

(script command_script course3
	(cs_fly_to "vset/p0")
	(cs_fly_to "vset/p1")
	(cs_fly_to "vset/p2")
	(cs_fly_to "vset/p3")
)

(script static void party
	(sleep_until
		(begin
			(effect_new_random "objects\characters\grunt\fx\grunt_birthday_party" "cset")
			(effect_new_random "objects\characters\grunt\fx\grunt_birthday_party" "sset")
			(effect_new_random "objects\characters\grunt\fx\grunt_birthday_party" "vset")
			(effect_new_random "objects\characters\grunt\fx\grunt_birthday_party" "bset")
			false
		)
	1 (* 10 30))
)

(script static void beh_test
	(ai_place "sq0/sp0")
)