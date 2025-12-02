<h1 align="center">Night Vision Goggles</h1>
<div align="center">
<a href="https://github.com/MS-crew/NightVisionGoggles/releases"><img src="https://img.shields.io/github/downloads/MS-crew/NightVisionGoggles/total?style=for-the-badge&logo=githubactions&label=Downloads" href="https://github.com/MS-crew/NightVisionGoggles/releases" alt="GitHub Release Download"></a>
<a href="https://github.com/MS-crew/NightVisionGoggles/releases"><img src="https://img.shields.io/badge/Build-1.0.0-brightgreen?style=for-the-badge&logo=gitbook" href="https://github.com/MS-crew/NightVisionGoggles/releases" alt="GitHub Releases"></a>
<a href="https://github.com/MS-crew/NightVisionGoggles/blob/master/LICENSE.txt"><img src="https://img.shields.io/badge/Licence-GPL_3.0-blue?style=for-the-badge&logo=gitbook" href="https://github.com/MS-crew/NightVisionGoggles/blob/master/LICENSE.txt" alt="General Public License v3.0"></a>
<a href="https://github.com/ExMod-Team/EXILED"><img src="https://img.shields.io/badge/Exiled-9.10.0-red?style=for-the-badge&logo=gitbook" href="https://github.com/ExMod-Team/EXILED" alt="GitHub Exiled"></a>

A standard-issue night-vision device designed by the SCP Foundation for use in dark or low-visibility environments. When activated, the goggles amplify ambient light and provide the user with clear visibility inside unlit areas of the facility.
This item as a practical tool for improving visibility during containment and security operations.
</div>

## Installation

1. Download the release file from the GitHub page [here](https://github.com/MS-crew/NightVisionGoggles/releases).
2. Extract the contents into your `\AppData\Roaming\EXILED\Plugins` directory.
3. Restart your server for creating default config file.
4. Configure the plugin according to your server’s needs using the provided settings.
5. Restart your server to apply the changes.

## Feedback and Issues
This is the initial release of the plugin. We welcome any feedback, bug reports, or suggestions for improvements.

- **Report Issues:** [Issues Page](https://github.com/MS-crew/NightVisionGoggles/issues)
- **Contact:** [discerrahidenetim@gmail.com](mailto:discerrahidenetim@gmail.com)

Thank you for using our plugin and helping us improve it!
## Default Config
```yml
is_enabled: true
debug: false
night_vision_insentity: 5
# Simulate the temporary darkness when wearing the glasses
simulate_temporary_darkness: true
# Wearing time (default 5)
override_wearing_time: false
wearing_time: 1
# Removal time (default 5.1)
override_wearing_off_time: false
wearing_off_time: 1
n_v_g:
  id: 757
  weight: 1
  name: 'Night Vision Goggles'
  description: 'A night-vision device (NVD), also known as a Night-Vision goggle (NVG), is an optoelectronic device that allows visualization of images in low levels of light, improving the user''s night vision.'
  spawn_properties:
    limit: 0
    dynamic_spawn_points: []
    static_spawn_points: []
    role_spawn_points: []
    room_spawn_points: []
    locker_spawn_points: []
  scale:
    x: 1
    y: 1
    z: 1
fake_light_settings:
  range: 50
  intensity: 70
  color:
    r: 0
    g: 1
    b: 0
    a: 1
  shadow_type: None
```
