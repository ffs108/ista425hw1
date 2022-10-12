
Assignment 1 -- ISTA 425                                            
========================================================================
+	Daniel Shevelev						       +
+	Koan Zhang						       +
+	Francisco Figueroa					       +
========================================================================

In this homework 1 assignment we have implemented the following upgrades to the base code:

* DeltaTime based rate-limited fire
	|- The amount of torpedos fired is no longer bound to the framerate the game runs on.
	   Instead it is bound to actual time elapsed, making the firerate standard across 
	   any system it runs on to be tied to seconds elapsed. 
* LERP-based guidance system
	|- The torpedo fired uses linear interpolation to decide its direction of travel 
	   between two points represented in the game world as to Vector3 objects. This also
	   allows for a more reliable impact tracker to be implemented on the space station 
	   object in order to produce a reliable impact tracker.
* Vector Reflection based torpedo reflections
	|- On impact the torpedo object will not thud into nothingness, instead it will
	   actually reflect on the surface of the space station using the reflection of the
	   incoming torpedo(using it's vector representation) and calculating the prime of
	   said vector which would be it's reflection upon the surface of impact.
* Bug follows the action
	|- The bug enemy represented on screen actually follows actions set upon the game 
	   world and rotates to face fired projectiles using the cross + dot product 
	   algorithm. The bug faces fired projects in sequential (order of fire) until they
	   they "run of time".
___________________________________________________________________________________________

The homework 1 assignment also had the following upgrades which were implemented:

* Damper based spaceship inertia
	|- The spaceship object gathers inertia on movements done in both transposive and 
	   rotational manners. This results in the spaceship slightly moving in either manner
	   after the player releases the movement input resulting in a more dynamic spaceship.
* World boundaries
	|- The world is constrained to the visible space available to game. This applies only
	   to torpedos objects as the spaceship object is free to roam outside these
	   boundaries. Torpedo objects are also able to reflect off said boundaries as they
	   are able to be modified with the "targetable" tag.
* Killing the bug
	|- The space bug object is able to snuffed of its existence by torpedo objects. There
	   is a FadeTime variable that sets how long it takes to have the bug enemy fade into
	   oblivion. 
