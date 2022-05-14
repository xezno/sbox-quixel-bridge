# Quixel Bridge plugin for s&box

Quickly and easily import Quixel assets using Quixel Bridge.

## Demo

https://user-images.githubusercontent.com/12881812/163437857-7d42e4f6-4533-4c68-a661-aec74a4c976a.mp4

## Disclaimer

Please be aware that the 'free' Quixel tier is for use with Unreal Engine only.
This tool should only be used with either the 'Personal', 'Indie', or 'Studio' Quixel tiers.
More information about these is located [here](https://quixel.com/pricing).

I am not affiliated with Quixel, Epic Games, or Facepunch.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

## Instructions

### Bridge Setup

1. Open Quixel Bridge (update to latest version if it asks you to)
2. Open "Edit" -> "Export Settings"
3. Change "Export Target" to "Custom Socket Export"
4. Open the "Textures" tab in Export Settings
5. Select "TGA" as format (not actually necessary, but seems to be faster - PNG and JPG are both also compatible, but will
compile slower)
6. Exit out of "Export Settings"

### s&box Setup

1. Download this repo
2. Extract it somewhere permanent
3. Add it through the s&box addon manager
4. The "Quixel" menu should appear in the s&box editor. Click "Start Bridge Plugin" to open Bridge and start the plugin.
