export class HaySample{
    id:number;
    name:string;
    hayTypeDetailsId:number;
}

export const HayType = [{
    id: 1,
    name: 'Grass Hay'

},
{
    id: 2,
    name: 'Mixed Hay'
},
{
    id: 3,
    name: 'Legume Hay'
},
{
    id: 4,
    name: 'Haylage'
},
{
    id: 5,
    name: 'Fermented Corn Silage'
},
{
    id: 6,
    name: 'Unfermented Corn Silage'
}

]

export const HayTypeDetails = [
    {
        id: 1,
        name: 'Alfalfa/Grass',
        hayTypeId: 1
    },
    {
        id: 2,
        name: 'Alfalfa(Lower Lignin, Lower Sugars)',
        hayTypeId: 1
    },
    {
        id: 3,
        name: 'Alfalfa/Oats',
        hayTypeId: 1
    },
    {
        id: 4,
        name: 'Bahia',
        hayTypeId: 1
    },
    {
        id: 5,
        name: 'Bermudagrass',
        hayTypeId: 1
    },
    {
        id: 6,
        name: 'Brome',
        hayTypeId: 2
    },
    {
        id: 7,
        name: 'Canola',
        haytypeId: 2
    },
    {
        id: 8,
        name: 'Clover/Grass',
        hayTypeId: 2
    },
    {
        id: 9,
        name: 'Pea',
        hayTypeId: 3
    },
    {
        id: 10,
        name: 'Peanut',
        hayTypeId: 3
    },
    {
        id: 11,
        name: 'Barley',
        hayTypeId: 4
    },
    {
        id: 12,
        name: 'BMRTraits',
        hayTypeId: 4
    },
    {
        id: 13,
        name: 'Legumes',
        hayTypeId: 4
    },
    {
        id: 14,
        name: 'BMRTraits',
        hayTypeId: 5
    },
    {
        id: 15,
        name: 'Forage Cut To All Stages',
        hayTypeId: 5
    },
    {
        id: 16,
        name: 'Fresh Cut',
        hayTypeId: 6
    },
    {
        id: 17,
        name: 'Some grains by product',
        hayTypeId: 6
    }
]